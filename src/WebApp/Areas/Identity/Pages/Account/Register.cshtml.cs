using System.Text.Encodings.Web;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyeShop.Infrastructure.Identity;
using MyeShop.WebApp.Areas.Identity.Pages.Account.Models;

namespace MyeShop.WebApp.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;

    public RegisterModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<RegisterModel> logger, IEmailSender emailSender)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _emailSender = emailSender;
    }

    [BindProperty]
    public required RegisterInputModel Input { get; set; }

    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (ModelState.IsValid)
        {
                _logger.LogInformation("Is Valid!");

            var user = new ApplicationUser { UserName = Input?.Email, Email = Input?.Email };
            var result = await _userManager.CreateAsync(user, Input?.Password!);
                _logger.LogInformation($"Result: {result.Succeeded}, user: {user} ");

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { user.Id, code = code },
                    protocol: Request.Scheme
                );

                Guard.Against.Null(callbackUrl, nameof(callbackUrl));
                await _emailSender.SendEmailAsync(
                    Input!.Email!, 
                    "Confirm Email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> Clicking here!</a>"
                );

                await _signInManager.SignInAsync(user, isPersistent: true);

                return LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        // If we got this far, something failed, redisplay form
        return Page();
    }

}