using System.ComponentModel.DataAnnotations;

namespace MyeShop.WebApp.ViewModels.Manage;

public class IndexViewModel
{
    public string? UserName { get; set; }
    public bool IsEmailConfirmed { get; set; }
    
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    [Display(Name ="Phone Number")]
    public string? PhoneNumber { get; set; }

    public string? StatusMessage { get; set; }
}