﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceLayer.Services;

public static class SeedData
{
    public static async Task SeedRoleAsync(IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "Moderator" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

        }
    }

    public static async Task SeedUserAsync(IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string userName = "AdminA";
            string email = "Admin@gmail.com";
            string password = "Admin123.";

            if (await userManager.FindByEmailAsync(email) is null)
            {
                var user = new IdentityUser
                {
                    UserName = userName,
                    Email = email
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Check if the role exists, if not, create it
                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        var role = new IdentityRole("Admin");
                        await roleManager.CreateAsync(role);
                    }

                    // Assign the user to the role
                    await userManager.AddToRoleAsync(user, "Admin");
                }

            }
        }
    }
}
