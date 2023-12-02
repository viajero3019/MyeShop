using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebShop.Areas.Identity.Pages.Account.Models;

namespace WebShop.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    

    [BindProperty]
    public required LoginInputModel Input { get; set;}

    public string? ReturnUrl{ get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }
}