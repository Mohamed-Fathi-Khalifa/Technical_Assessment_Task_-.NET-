using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Domain.Enums.TaskStatus;

namespace Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // No dummy data seeded as per production requirements.
        // Users must register manually via the /api/v1/auth/register endpoint.
        await Task.CompletedTask;
    }
}
