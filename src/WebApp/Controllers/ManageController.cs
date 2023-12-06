using System.Text.Encodings.Web;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyeShop.Infrastructure.Identity;
using MyeShop.WebApp.ViewModels.Manage;
using MyeShop.Web.Extensions;
using MyeShop.ApplicationCore.Interfaces;
using MyeShop.WebApp.Configuration;
using System.Text.Json;
using MyeShop.ApplicationCore;

namespace MyeShop.WebApp.Controllers;

public class ManageController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly UrlEncoder _urlEncoder;
    private readonly IConfiguration _configuration;

    // private readonly IAppLogger<ManageController> _logger;

    private const string AuthenticationUriFormat = "otpauth://top/{0}:{1}&issuer={0}&digits=6";
    private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

    public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, UrlEncoder urlEncoder, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _urlEncoder = urlEncoder;
        _configuration = configuration;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    [HttpGet]
    public async Task<IActionResult> MyAccount()
    {
        var user = await GetUserAsync();

        var model = new IndexViewModel
        {
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsEmailConfirmed = user.EmailConfirmed,
            StatusMessage = StatusMessage
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MyAccount(IndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await GetUserAsync();

        var email = user.Email;
        if (model.Email != email)
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
            if (!setEmailResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
            }
        }

        var phoneNumber = user.PhoneNumber;
        if (model.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
            }
        }

        StatusMessage = "Your profile has been updated";
        return RedirectToAction(nameof(MyAccount));
    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        var user = await GetUserAsync();

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToAction(nameof(SetPassword));
        }

        var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await GetUserAsync();

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);
        if (!changePasswordResult.Succeeded)
        {
            AddErrors(changePasswordResult);
            return View(model);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        // _logger.LogInformation("User changed their password successfully.");
        StatusMessage = "Your password has been changed.";

        return RedirectToAction(nameof(ChangePassword));
    }


    [HttpGet]
    public async Task<IActionResult> SetPassword()
    {
        var user = await GetUserAsync();
        var hasPassword = await _userManager.HasPasswordAsync(user);

        if (hasPassword)
        {
            return RedirectToAction(nameof(ChangePassword));
        }

        var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await GetUserAsync();
        var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword!);
        if (!addPasswordResult.Succeeded)
        {
            AddErrors(addPasswordResult);
            return View(model);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        StatusMessage = "Your password has been set.";

        return RedirectToAction(nameof(SetPassword));
    }

    public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await GetUserAsync();

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
        Guard.Against.Null(callbackUrl, nameof(callbackUrl));
        
        var email = user.Email;
        if (email == null)
        {
            throw new ApplicationException($"No email associated with user {user.UserName}'.");
        }

        var mailSettings =_configuration.GetSection("Smtp").Get<MailSettings>();
        await _emailSender.SendEmailConfirmationAsync(email, callbackUrl, mailSettings!);

        StatusMessage = "Verification email sent. Please check your email.";
        return RedirectToAction(nameof(MyAccount));
    }

    private async Task<ApplicationUser> GetUserAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }
        return user;
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}