using FirebaseAdmin.Messaging;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using BeatSportsAPI.Application.Common.Models;
using FirebaseMessagingException = BeatSportsAPI.Application.Common.Exceptions.FirebaseMessagingException;

namespace BeatSportsAPI.Infrastructure.Notification;
public class NotificationService : INotificationService
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public NotificationService(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<string> GetUserDeviceToken(Guid userId)
    {
        var customer = await _beatSportsDbContext.Customers
            .Where(c => c.Id == userId)
            .Select(c => new { c.AccountId })
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            throw new NotFoundException("Customer not found.");
        }

        var deviceToken = await _beatSportsDbContext.DeviceTokens
            .Where(dt => dt.AccountId == customer.AccountId)
            .Select(dt => dt.Token)
            .FirstOrDefaultAsync();

        if (deviceToken == null)
        {
            throw new NotFoundException("Device token not found.");
        }

        return deviceToken;
    }

    public async Task<string> SendNotificationAsync(NotificationModels notification)
    {
        //Check token from user is valid or not
        var token = await GetUserDeviceToken(notification.UserId); 

        if (string.IsNullOrEmpty(token))
        {
            throw new NotFoundException("Device token not found for the user.");
        }
        //if valid, create Notification model
        var message = new Message()
        {
            Token = token,
            Notification = new FirebaseAdmin.Messaging.Notification()
            {
                Title = notification.Title,
                Body = notification.Body,
            }
        };  
        //Send noti via firebase
        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        if(response == null)
        {
            throw new FirebaseMessagingException("An error is occured");
        }
        return response;
    }
}
