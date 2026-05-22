using Application.Common;
using Application.Features.Projects.Commands;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.UnitTests.Features.Projects;

public class CreateProjectCommandHandlerTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a Moq DbSet that captures Add() calls into the supplied list,
    /// allowing us to assert on the saved entity after the handler runs.
    /// </summary>
    private static Mock<DbSet<T>> BuildMockDbSet<T>(List<T> store) where T : class
    {
        var mock = new Mock<DbSet<T>>();
        mock.Setup(d => d.Add(It.IsAny<T>()))
            .Callback<T>(item => store.Add(item));
        return mock;
    }

    // ── Success Scenario ─────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ShouldCreateProject_WhenCommandIsValid()
    {
        // Arrange
        const int currentUserId = 42;
        var projectStore        = new List<Project>();

        var mockDbSet   = BuildMockDbSet(projectStore);
        var mockContext = new Mock<IApplicationDbContext>();
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);
        mockContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.Setup(u => u.UserId).Returns(currentUserId);

        var handler = new CreateProjectCommandHandler(mockContext.Object, mockCurrentUser.Object);
        var command = new CreateProjectCommand("My Test Project", "A detailed description");

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert — response
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("My Test Project",       response.Data.Name);
        Assert.Equal("A detailed description", response.Data.Description);
        Assert.Equal(currentUserId,            response.Data.UserId);

        // Assert — side effects
        Assert.Single(projectStore);
        Assert.Equal(currentUserId, projectStore[0].UserId);
        mockDbSet.Verify(d => d.Add(It.IsAny<Project>()), Times.Once);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Failure Scenario ─────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenDatabaseThrows()
    {
        // Arrange
        var mockDbSet   = new Mock<DbSet<Project>>();
        var mockContext = new Mock<IApplicationDbContext>();
        mockContext.Setup(c => c.Projects).Returns(mockDbSet.Object);
        mockContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Simulated database failure."));

        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.Setup(u => u.UserId).Returns(1);

        var handler = new CreateProjectCommandHandler(mockContext.Object, mockCurrentUser.Object);
        var command = new CreateProjectCommand("Failing Project", null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("Simulated database failure.", ex.Message);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
