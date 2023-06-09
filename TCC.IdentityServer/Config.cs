using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;

namespace TCC.IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("TCC-auth", "TCC.Auth")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            var url = configuration.GetValue<string>("tcc-front");

            return new List<Client>
            {
                // Hybrid Flow = OpenId Connect + OAuth
                // To use both Identity and Access Tokens
                new Client
                {
                    ClientId = "TCC-auth",
                    ClientName = "TCC autenticação",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowOfflineAccess = true,
                    RequireConsent = false,

                    RedirectUris = { url + "/signin-oidc", url + "/home"  },
                    PostLogoutRedirectUris = { url + "/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TCC-auth"
                    },
                },              
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "administrador",
                    Password = "administrador",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Kevin Administrador"),
                        new Claim(JwtClaimTypes.Role, "administrador"),
                        new Claim(JwtClaimTypes.GivenName, "Kevin Gomes"),
                        new Claim(JwtClaimTypes.Email, "administrador@tcc.com.br"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "Cliente",
                    Password = "Cliente",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Maria Cliente"),
                        new Claim(JwtClaimTypes.Role, "Cliente"),
                        new Claim(JwtClaimTypes.GivenName, "Maria"),
                        new Claim(JwtClaimTypes.Email, "Cliente@tcc.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "3",
                    Username = "Funcionario",
                    Password = "Funcionario",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "João Funcionario"),
                        new Claim(JwtClaimTypes.Role, "Funcionario"),
                        new Claim(JwtClaimTypes.GivenName, "João"),
                        new Claim(JwtClaimTypes.Email, "Funcionario@tcc.com")
                    }
                }
            };
        }
    }
}