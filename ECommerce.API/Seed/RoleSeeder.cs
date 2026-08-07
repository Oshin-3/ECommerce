using Microsoft.AspNetCore.Identity;
using ECommerce.Domain.Contants;
using ECommerce.Domain.Entities;

namespace ECommerce.API.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRoleAsync(RoleManager<IdentityRole<Guid>> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            //for Admin
            if(!await roleManager.RoleExistsAsync("Admin"))
            {
                //create the role
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Name = Roles.Admin
                });
            }

            //for Customer
            if(!await roleManager.RoleExistsAsync("Customer"))
            {
                //create role
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Name = Roles.Customer
                });
            }

            var adminSection = configuration.GetSection("DefaultAdmin");
            var adminEmail = adminSection["Email"];
            var adminPassword = adminSection["Password"];
            
            //check if admin exits 
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if(adminUser == null)
            {
                //seeding the initial admin role
                var newAdminUser = new ApplicationUser
                {
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdminUser, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newAdminUser, Roles.Admin);
            }

           
            
        }

    }
}
