using Andromeda.Data;
using Microsoft.EntityFrameworkCore;

namespace Andromeda.Exceptions;

public static class DatabaseMigrationExtensions
{
    public static async Task ApplyMigrationsAsync(
        this IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync();
    }
}
