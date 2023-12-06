using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MyeShop.ApplicationCore;
using MyeShop.ApplicationCore.Interfaces;
using MyeShop.WebApp.Configuration;

namespace MyeShop.WebApp.Extensions;

public static class EmailSenderExtensions
{
    public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link, MailSettings mailSettings)
    {
        Console.WriteLine("--> SendEmailConfirmationAsync()");

        return emailSender.SendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>", mailSettings);
    }
}