using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4Test.IDP.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4Test.IDP.Infrastructure
{
    public class ProfileService : IdentityServer4.AspNetIdentity.ProfileService<ApplicationUser>
    {
        protected readonly UserManager<ApplicationUser> _UserManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _ClaimsFactory;

        public ProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
            : base(userManager, claimsFactory)
        {
            _UserManager = userManager;
            _ClaimsFactory = claimsFactory;
        }

        public async override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _UserManager.GetUserAsync(context.Subject);
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Email, user.Email.ToString()),
                new Claim("username", user.UserName.ToString()),
                new Claim(ClaimTypes.Role, "superAdmin"),
                new Claim("test_claim", "3333")
        };

            context.IssuedClaims.AddRange(claims);

            //var roles = await _UserManager.GetRolesAsync(user);

            //foreach (var role in roles)
            //{
            //    context.IssuedClaims.Add(new Claim(JwtClaimTypes.Role, role));
            //}
            
        }

        public async override Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _UserManager.GetUserAsync(context.Subject);

            context.IsActive = (user != null) && user.IsActive;
        }
    }
}
