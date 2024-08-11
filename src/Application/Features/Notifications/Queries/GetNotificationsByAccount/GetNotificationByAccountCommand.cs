using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Notifications.Queries.GetNotificationsByAccount;
public class GetNotificationByAccountCommand : IRequest<List<NotificationResponse>>
{
    public string AccountId { get; set; }
}

public class GetNotificationByAccountHandler : IRequestHandler<GetNotificationByAccountCommand, List<NotificationResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetNotificationByAccountHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<List<NotificationResponse>> Handle(GetNotificationByAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _dbContext.Accounts.Where(a => a.Id == Guid.Parse(request.AccountId)).SingleOrDefaultAsync();
        var notifications = await _dbContext.Notifications
            .Where(n => n.AccountId == Guid.Parse(request.AccountId))
            .OrderByDescending(n => n.Created).ToListAsync();

        var response = new List<NotificationResponse>();
        foreach (var notification in notifications)
        {
            var notificationResponse = new NotificationResponse
            {
                NotificationId = notification.Id,
                AccountId = notification.AccountId,
                BookingId = notification.BookingId,
                FullName = account.FirstName.Trim() + " " + account.LastName.Trim(),
                AccountImage = account.ProfilePictureURL,
                Title = notification.Title,
                Message = notification.Message,
                Created = notification.Created,
                IsRead = notification.IsRead,
                Type = notification.Type
            };
            response.Add(notificationResponse);
        }

        return response;
    }
}
