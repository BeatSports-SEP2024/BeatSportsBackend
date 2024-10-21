using MailKit.Net.Smtp;
using BeatSportsAPI.Application.Common.Interfaces;
using MimeKit;
using BeatSportsAPI.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BeatSportsAPI.Infrastructure.Services.SendEmail;
public class EmailService : IEmailService
{
    private readonly IMemoryCache _memoryCache;

    public EmailService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string html)
    {
        var emailConfig = EmailConfiguration.LoadFromConfiguration();

        //Create a mail format when send email
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(emailConfig.SenderName, emailConfig.Sender));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = html };

        //Config connect to smtp 
        using var smtp = new SmtpClient();
        smtp.Connect(emailConfig.MailServer, emailConfig.MailPort, MailKit.Security.SecureSocketOptions.StartTls);
        smtp.Authenticate(emailConfig.Sender, emailConfig.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}