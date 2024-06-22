namespace Identity.Services;

public interface IEmailService
{
    Task<bool> SendMailAsync(MailData mailData);
}
