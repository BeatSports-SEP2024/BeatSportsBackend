using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;

namespace BeatSportsAPI.Application.Common.Interfaces;
public interface INotificationService
{
    //Token is known as user device token get by Userid
    //Message title and message body
    Task<string> SendNotificationAsync(NotificationModels notification);
    //Get Device token depend on Id of User
    Task<string> GetUserDeviceToken(Guid userId);
}