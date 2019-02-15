using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4Test.IDP.Models;
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4Test.IDP.Data
{
    public class SeedConfiguration
    {
        private readonly ApplicationDbContext DbContext;
        private readonly UserManager<ApplicationUser> UserIdentityManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private readonly IServiceProvider ServiceProvider;

        public SeedConfiguration(ApplicationDbContext _dbContext,
            UserManager<ApplicationUser> userManager, IServiceProvider serviceProvider,
            RoleManager<IdentityRole> roleManager)
        {
            DbContext = _dbContext;
            UserIdentityManager = userManager;
            ServiceProvider = serviceProvider;
            RoleManager = roleManager;
        }

        public async Task SeedData()
        {
            await SeedSuperAdmin();
            await SeedClientsAndResources();
            //await SeedTenant();
        }

        private async Task SeedSuperAdmin()
        {
            var superAdminUser = await UserIdentityManager.FindByEmailAsync("superAdmin@tenpearls.com");

            if (superAdminUser == null)
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = "superAdmin",
                    Email = "superAdmin@tenpearls.com",
                    PhoneNumber = "121212121",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                };

                var roleResult = await RoleManager.FindByNameAsync(applicationUser.UserName);
                if (roleResult == null)
                {
                    roleResult = new IdentityRole("superAdmin");
                    await RoleManager.CreateAsync(roleResult);
                }

                var roleClaimList = (await RoleManager.GetClaimsAsync(roleResult)).Select(p => p.Type);
                if (!roleClaimList.Contains("ManagerPermissions"))
                {
                    await RoleManager.AddClaimAsync(roleResult, new Claim("ManagerPermissions", "true"));
                }


                var result = await UserIdentityManager.CreateAsync(applicationUser, "Pass123$");

                //var claims = new List<Claim>
                //{
                //        new Claim(JwtClaimTypes.Email, applicationUser.Email.ToString()),
                //        new Claim("username", applicationUser.UserName.ToString()),
                //        new Claim(ClaimTypes.Role, "superAdmin"),
                //        new Claim("phone_number", "123455677")
                //};

                //UserIdentityManager.AddClaimsAsync(applicationUser, claims).Wait();

                if (result.Succeeded)
                    await UserIdentityManager.AddToRoleAsync(applicationUser, "SuperAdmin");
            }
        }

        private async Task SeedClientsAndResources()
        {
            using (var scope = ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext>();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients().ToList())
                        context.Clients.Add(client.ToEntity());

                    await context.SaveChangesAsync();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources().ToList())
                        context.IdentityResources.Add(resource.ToEntity());

                    await context.SaveChangesAsync();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApis().ToList())
                        context.ApiResources.Add(resource.ToEntity());

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
