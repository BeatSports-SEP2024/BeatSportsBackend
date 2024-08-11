using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Notifications.Commands.Update;
public class UpdateNotificationIsReadCommand : IRequest<BeatSportsResponse>
{
    public string NotificationId { get; set; }
}

public class UpdateNotificationIsReadHandler : IRequestHandler<UpdateNotificationIsReadCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateNotificationIsReadHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<BeatSportsResponse> Handle(UpdateNotificationIsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _dbContext.Notifications.Where(n => n.Id == Guid.Parse(request.NotificationId)).SingleOrDefaultAsync();
        if (notification == null)
        {
            throw new NotFoundException("Không tìm thấy thông báo trên"); 
        }
        notification.IsRead = true;
        _dbContext.Notifications.Update(notification);
        await _dbContext.SaveChangesAsync();

        return new BeatSportsResponse
        {
            Message = "Update Successfully"
        };
    }
}
