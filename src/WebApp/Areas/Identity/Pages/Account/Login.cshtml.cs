using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MyeShop.Infrastructure.Identity;
using MyeShop.WebApp.Areas.Identity.Pages.Account.Models;

namespace MyeShop.WebApp.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public required LoginInputModel Input { get; set;}

    public string? ReturnUrl{ get; set; }

    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if(!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl = returnUrl ?? Url.Content("~/");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if(ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(Input.Email!, Input.Password!, false, true);
            Console.WriteLine("returnUrl: " + returnUrl);
            Console.WriteLine("--> Entered, Result: " + result.ToString());
            if(result.Succeeded)
            {
                _logger.LogInformation("User logged in");
                return LocalRedirect(returnUrl);
            }
        }
            Console.WriteLine("--> Not valid");

        // If we got this far, something failed, redisplay form
        return Page();
    }
}