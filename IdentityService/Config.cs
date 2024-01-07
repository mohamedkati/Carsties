using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("auctionApp", "Auction app full access"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "postman",
                ClientName = "postman",
                ClientSecrets = { new Secret("NotASecret".Sha256()) },
                AllowedScopes = { "openid", "profile", "auctionApp" },
                RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
                AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
            },
            new Client
            {
                ClientId = "NextApp",
                ClientName = "NextApp",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "openid", "profile", "auctionApp" },
                RedirectUris = { "http://localhost:3000/api/auth/callback/id-server" },
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                AllowOfflineAccess = true,
                RequirePkce = false,
                AccessTokenLifetime = 3600 * 24 * 30,
                AlwaysIncludeUserClaimsInIdToken = true
            },
        };
}