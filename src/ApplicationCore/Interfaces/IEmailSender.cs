using System.Threading.Tasks;

namespace MyeShop.ApplicationCore.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message, MailSettings mailSettings);
}