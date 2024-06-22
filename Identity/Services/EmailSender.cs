using System.Net.Mail;
using System.Net;
using Identity.Utils;
using Microsoft.Extensions.Options;

namespace Identity.Services;

public class EmailSender : IEmailService
{
    private readonly MailSettings mailSettings;

    public EmailSender(IOptions<MailSettings> mailSettingsOptions)
    {
        mailSettings = mailSettingsOptions.Value;
    }

    public async Task<bool> SendMailAsync(MailData mailData)
    {
        try
        {
            // Create a MailMessage object
            MailMessage mail = new();
            mail.From = new MailAddress(mailSettings.SenderEmail);
            mail.To.Add(mailData.EmailToId);
            mail.Subject = mailData.EmailSubject;
            mail.Body = mailData.EmailBody;
            mail.IsBodyHtml = false; // Set to true if the body contains HTML

            // Set up the SMTP client
            // Use your SMTP server and port
            SmtpClient smtpClient = new(mailSettings.Server, mailSettings.Port) 
            {
                Credentials = new NetworkCredential(mailSettings.UserName,
                                                    mailSettings.Password),
                EnableSsl = true // Use SSL if required by your SMTP server
            };

            // Send the email
            smtpClient.Send(mail);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}
