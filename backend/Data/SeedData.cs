using Andromeda.Entities;
using Andromeda.Enums;
using Microsoft.AspNetCore.Identity;

namespace Andromeda.Data;

public static class SeedData
{

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        #region Roles

            foreach (var role in Enum.GetNames<Roles>())
            {
                var exists = await roleManager.RoleExistsAsync(role);

                if (!exists)
                {
                    await roleManager.CreateAsync(new Role(role));
                }
            }

        #endregion

        #region Owner

        var owners = new[]
        {
                new
                {
                    UserName = "Koorosh",
                    Email = "koorosh@andromeda.local",
                    FirstName = "Koorosh",
                    LastName = "Soleymani",
                    Password = "Koorosh@1387"
                },
                new
                {
                    UserName = "Radin",
                    Email = "radin@andromeda.local",
                    FirstName = "Radin",
                    LastName = "Khodadadi",
                    Password = "Radin@1387"
                }
            };

        foreach (var item in owners)
        {
            var user = await userManager.FindByEmailAsync(item.Email);

            if (user != null)
                continue;

            user = new User
            {
                UserName = item.UserName,
                Email = item.Email,
                FirstName = item.FirstName,
                LastName = item.LastName,
                State = UserState.Active,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, item.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, nameof(Roles.Owner));
            }
        }

        #endregion
    }
}