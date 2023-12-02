using System.ComponentModel.DataAnnotations;

namespace MyeShop.WebApp.Areas.Identity.Pages.Account.Models;

public class LoginInputModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}