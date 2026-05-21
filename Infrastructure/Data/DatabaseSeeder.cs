using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Only seed when the database is completely empty
        if (await context.Users.AnyAsync())
            return;

        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            // Placeholder hash — replace with real BCrypt hash in production
            PasswordHash = "AQAAAAIAAYagAAAAEPlaceholderHashForDevelopmentOnly=="
        };

        var project = new Project
        {
            Name = "Sample Project",
            Description = "A seed project created for development and testing purposes.",
            CreatedAt = DateTime.UtcNow,
            User = user
        };

        var task = new TaskItem
        {
            Title = "Initial Task",
            Description = "This is a seeded sample task.",
            Status = TaskStatus.Todo,
            Priority = TaskPriority.Medium,
            DueDate = DateTime.UtcNow.AddDays(7),
            Project = project
        };

        context.Users.Add(user);
        context.Projects.Add(project);
        context.Tasks.Add(task);

        await context.SaveChangesAsync();
    }
}
