using AfrawyStore.Domain.Entities;
using AfrawyStore.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AfrawyStore.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure database is created/migrated
        if (context.Database.IsSqlServer())
        {
            await context.Database.MigrateAsync();
        }

        // Check if any users exist
        if (!await context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                Username = "admin",
                FullName = "مدير النظام",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var hasher = new PasswordHasher<User>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin123");

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
