using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Identity.Services;

public class EmailService(IOptions<MailSettings> mailSettingsOptions, 
                          IConfiguration config) : IEmailService
{
    private readonly MailSettings mailSettings = mailSettingsOptions.Value;

    public async Task<bool> SendMailAsync(MailData mailData)
    {
        try
        {
            using MimeMessage emailMessage = new();
            MailboxAddress emailFrom = new(mailSettings.SenderName, mailSettings.SenderEmail);
            emailMessage.From.Add(emailFrom);
            MailboxAddress emailTo = new(mailData.EmailToName, mailData.EmailToId);
            emailMessage.To.Add(emailTo);

            // you can add the CCs and BCCs here.
            //emailMessage.Cc.Add(new MailboxAddress("Cc Receiver", "cc@example.com"));
            //emailMessage.Bcc.Add(new MailboxAddress("Bcc Receiver", "bcc@example.com"));

            emailMessage.Subject = mailData.EmailSubject;

            BodyBuilder emailBodyBuilder = new()
            {
                TextBody = mailData.EmailBody
            };

            emailMessage.Body = emailBodyBuilder.ToMessageBody();
            //this is the SmtpClient from the Mailkit.Net.Smtp namespace,
            //not the System.Net.Mail one
            mailSettings.UserName = "dilipnc96@gmail.com";
            mailSettings.Password = config["Password"];
            using SmtpClient mailClient = new();
            await mailClient.ConnectAsync(mailSettings.Server, mailSettings.Port, 
                                          MailKit.Security.SecureSocketOptions.StartTls);
            await mailClient.AuthenticateAsync(mailSettings.UserName, mailSettings.Password);
            await mailClient.SendAsync(emailMessage);
            await mailClient.DisconnectAsync(true);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
