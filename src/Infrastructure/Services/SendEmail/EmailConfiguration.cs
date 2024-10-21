using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Infrastructure.Common;

namespace BeatSportsAPI.Infrastructure.Services.SendEmail;
public class EmailConfiguration
{
    public string Sender { get; set; }
    public string SenderName { get; set; }
    public int MailPort { get; set; }
    public string MailServer { get; set; }
    public string Password { get; set; }

    public static EmailConfiguration LoadFromConfiguration()
    {
        return new EmailConfiguration
        {
            Sender = GetJsonInAppSettingsExtension.GetJson("Email:Sender"),
            SenderName = GetJsonInAppSettingsExtension.GetJson("Email:SenderName"),
            MailPort = int.Parse(GetJsonInAppSettingsExtension.GetJson("Email:MailPort")),
            MailServer = GetJsonInAppSettingsExtension.GetJson("Email:MailServer"),
            Password = GetJsonInAppSettingsExtension.GetJson("Email:Password")
        };
    }
}