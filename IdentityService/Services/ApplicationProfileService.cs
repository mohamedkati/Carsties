using System.Security.Claims;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class ApplicationProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await userManager.GetUserAsync(context.Subject);
        if (user != null)
        {
            var existingClaims = await userManager.GetClaimsAsync(user);
            var claims = new Claim[]
            {
                new Claim("username", user.UserName ?? "")
            };
             context.IssuedClaims.AddRange(claims);
             context.IssuedClaims.AddRange(existingClaims);
        }
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        return Task.CompletedTask;
    }
}