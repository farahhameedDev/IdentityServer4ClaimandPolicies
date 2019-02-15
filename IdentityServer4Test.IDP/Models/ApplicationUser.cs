using Microsoft.AspNetCore.Identity;

namespace IdentityServer4Test.IDP.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int TenantId { get; set; }

        public bool IsActive { get; set; }
    }
}
