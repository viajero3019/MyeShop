using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MyeShop.ApplicationCore;
using MyeShop.ApplicationCore.Interfaces;

namespace MyeShop.Infrastructure.Services;

// This class is used by the application to send email for account confirmation and password reset.
// For more details see https://go.microsoft.com/fwlink/?LinkID=532713
public class EmailSender : IEmailSender
{
    static bool mailSent = false;
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task SendEmailAsync(string email, string subject, string message, MailSettings mailSettings)
    {
        // TODO: Wire this up to actual email sending logic via SendGrid, local SMTP, etc.
        Console.WriteLine("--> SendEmailAsync()");

        SmtpClient smtpClient = new SmtpClient();
        smtpClient.Host = mailSettings.Server!;
        smtpClient.Port = mailSettings.Port;
        smtpClient.UseDefaultCredentials = true;
        smtpClient.Credentials = new NetworkCredential(mailSettings.FromAddress!, mailSettings.Credentials!);
        smtpClient.EnableSsl = true;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        MailAddress from = new MailAddress(mailSettings.FromAddress!, mailSettings.FromAddress!, System.Text.Encoding.UTF8);
        MailAddress to = new MailAddress(email);
        MailMessage messageMail = new MailMessage(from, to);

        messageMail.Body = message;
        messageMail.BodyEncoding = System.Text.Encoding.UTF8;
        messageMail.Subject = subject;
        messageMail.SubjectEncoding = System.Text.Encoding.UTF8;

        smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

        string userState = subject;
        smtpClient.SendAsync(messageMail, userState);

        return Task.CompletedTask;
    }

    private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        // Get the unique identifier for this asynchronous operation.
        String token = (string)e.UserState;

        if (e.Cancelled)
        {
            Console.WriteLine("[{0}] Send canceled.", token);
        }
        if (e.Error != null)
        {
            Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
        }
        else
        {
            Console.WriteLine("Message sent.");
        }
        mailSent = true;
    }
}
