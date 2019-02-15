using IdentityServer4;
using IdentityServer4.Test;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityModel;

namespace IdentityServer4Test.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
              //  new IdentityResource("roles", new[] { "role" })
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            var claimTypes = new List<string>() {
                JwtClaimTypes.Email,
                JwtClaimTypes.Id,
                JwtClaimTypes.Role,
                "username"
            };

            return new List<ApiResource>
            {
                new ApiResource("COGS.Api", "My API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AlwaysSendClientClaims = true,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris           = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                    RequireConsent = false,
                    AccessTokenLifetime = 4800,

                    //for consent
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "COGS.Api"
                    },

                    AllowOfflineAccess = true
                }
            };
        }

        /// <summary>
        /// Define which uses will use this IdentityServer
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestUser> GetUsers()
        {
            return new []
            {
                new TestUser
                {
                    SubjectId = "10001",
                    Username = "edison@hotmail.com",
                    Password = "edisonpassword"
                },
                new TestUser
                {
                    SubjectId = "10002",
                    Username = "andy@hotmail.com",
                    Password = "andypassword"
                },
                new TestUser
                {
                    SubjectId = "10003",
                    Username = "leo@hotmail.com",
                    Password = "leopassword"
                }
            };
        }
    }
}
