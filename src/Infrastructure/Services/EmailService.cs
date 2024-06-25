using MailKit.Net.Smtp;
using BeatSportsAPI.Application.Common.Interfaces;
using MimeKit;
using BeatSportsAPI.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BeatSportsAPI.Infrastructure.Services;
public class EmailService : IEmailService
{
    private readonly IMemoryCache _memoryCache;

    public EmailService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string html)
    {
        var emailSender = GetJsonInAppSettingsExtension.GetJson("Email:Sender");
        var emailSenderName = GetJsonInAppSettingsExtension.GetJson("Email:SenderName");
        var mailPort = GetJsonInAppSettingsExtension.GetJson("Email:MailPort");
        var mailServer = GetJsonInAppSettingsExtension.GetJson("Email:MailServer");
        var password = GetJsonInAppSettingsExtension.GetJson("Email:Password");

        //Create a mail format when send email
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(emailSenderName, emailSender));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = html };

        //Config connect to smtp 
        using var smtp = new SmtpClient();
        smtp.Connect(mailServer, int.Parse(mailPort), MailKit.Security.SecureSocketOptions.StartTls);
        smtp.Authenticate(emailSender, password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}