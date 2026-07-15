using Andromeda.Entities;
using Andromeda.Enums;
using Microsoft.AspNetCore.Identity;

namespace Andromeda.Data;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        foreach (var role in Enum.GetNames<Roles>())
        {
            var exists = await roleManager.RoleExistsAsync(role);

            if (!exists)
            {
                await roleManager.CreateAsync(new Role(role));
            }
        }
    }
}