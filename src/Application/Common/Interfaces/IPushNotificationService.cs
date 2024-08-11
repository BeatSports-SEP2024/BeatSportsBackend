using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Interfaces;
public interface IPushNotificationService
{
    Task SendPushNotification(string expoPushToken, string title, string body, object data);
}
