using System.Security.Claims;
using Duende.IdentityServer.Models;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace IdentityService.Pages.Account.Register;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    [BindProperty] public RegisterViewModel Input { get; set; }
    [BindProperty] public bool RegisterSucced { get; set; }

    public Index(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult OnGet(string returnUrl)
    {
        RegisterSucced = false;
        Input = new RegisterViewModel() { ReturnUrl = returnUrl };

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        Log.Debug(Input.Button);
        RegisterSucced = false;
        if (Input.Button != "Register") return Redirect("~/");

        if (!ModelState.IsValid) return Page();
        
        var appUser = new ApplicationUser()
        {
            Email = Input.Email,
            UserName = Input.Username,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(appUser, Input.Password);
        
        Log.Debug(result.ToString());
        if (!result.Succeeded)
        {
            return Page();
        }
        
        Log.Debug("User created");
        
        var userClaims = await _userManager.AddClaimsAsync(appUser, new List<Claim>()
        {
            new(JwtClaimTypes.Name, Input.Username),
            new(JwtClaimTypes.GivenName, Input.Fullname),
        });
        
        Log.Debug("Claims created");
        RegisterSucced = true;

        return Page();
    }
}