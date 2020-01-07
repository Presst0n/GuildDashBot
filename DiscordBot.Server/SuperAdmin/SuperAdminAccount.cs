using DiscordBot.Server.Models;
using DiscordBot.Server.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DiscordBot.Server.SuperAdmin
{
    public class SuperAdminAccount
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public SuperAdminAccount(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public Task CreateSuperAdmin()
        {
            var userEmail = _configuration.GetSection("UserSettings").GetValue<string>("UserEmail");

            var user = _userManager.FindByEmailAsync(userEmail).Result;

            if (user is null)
            {
                var superUser = new IdentityUser()
                {
                    Email = userEmail,
                    UserName = userEmail,
                    EmailConfirmed = true
                };

                var tempPassword = _configuration.GetSection("UserSettings").GetValue<string>("UserPassword");

                var result = _userManager.CreateAsync(superUser, tempPassword).Result;

                if (result.Succeeded)
                {
                    var role = _roleManager.FindByNameAsync("Super Admin").Result;

                    if (role is null)
                    {
                        var newRole = new IdentityRole()
                        {
                            Name = "Super Admin"
                        };

                        var roleResult = _roleManager.CreateAsync(newRole).Result;

                        if (roleResult.Succeeded)
                        {
                            _userManager.AddToRoleAsync(superUser, "Super Admin");
                        }
                    }
                    else
                    {
                        _userManager.AddToRoleAsync(superUser, "Super Admin");
                    }

                    var existingUserClaims = _userManager.GetClaimsAsync(superUser).Result;

                    if (existingUserClaims.Count < 1)
                    {
                        List<Claim> AllClaims = new List<Claim>()
                        {
                            new Claim("Create Role", "true"),
                            new Claim("Edit Role", "true"),
                            new Claim("Delete Role", "true"),
                            new Claim("Manage Claims", "true")
                        };


                        var claimResult = _userManager.AddClaimsAsync(superUser, AllClaims).Result;
                    }
                    else
                    {

                        _userManager.AddClaimsAsync(superUser, existingUserClaims);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
