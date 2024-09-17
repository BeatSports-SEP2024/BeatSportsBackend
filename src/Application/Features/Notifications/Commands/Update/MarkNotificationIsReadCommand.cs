using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Notifications.Commands.Update;
public class MarkNotificationIsReadCommand : IRequest<BeatSportsResponse>
{
    public Guid AccountId { get; set; }
}

public class MarkNotificationIsReadCommandHandler : IRequestHandler<MarkNotificationIsReadCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public MarkNotificationIsReadCommandHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<BeatSportsResponse> Handle(MarkNotificationIsReadCommand request, CancellationToken cancellationToken)
    {
        var notifications = await _dbContext.Notifications.Where(n => n.AccountId == request.AccountId).ToListAsync();
        if (notifications == null || !notifications.Any())
        {
            return new BeatSportsResponse
            {
                Message = "Tất cả thông báo đã được đánh dấu đã đọc hết rồi!!"
            };
        }
        else
        {
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _dbContext.Notifications.Update(notification);
            }
            await _dbContext.SaveChangesAsync();

            return new BeatSportsResponse
            {
                Message = "Tất cả thông báo đã được đánh dấu đã đọc thành công !!!"
            };
        }
    }
}
