namespace MyeShop.ApplicationCore;

public class MailSettings
{
    public string? Server { get; set; }
    public int Port { get; set; }
    public string? FromAddress { get; set; }
    public string? Credentials { get; set; }
}