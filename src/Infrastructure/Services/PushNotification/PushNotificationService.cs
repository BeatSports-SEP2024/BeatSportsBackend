using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BeatSportsAPI.Infrastructure.Services.PushNotification;
public class PushNotificationService : IPushNotificationService
{
    private readonly HttpClient _httpClient;

    public PushNotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendPushNotification(string expoPushToken, string title, string body, object data)
    {
        var message = new
        {
            to = expoPushToken,
            sound = "default",
            title = title,
            body = body,
            data = data
        };

        var jsonMessage = JsonConvert.SerializeObject(message);
        var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://exp.host/--/api/v2/push/send", content);
        response.EnsureSuccessStatusCode();
    }
}

