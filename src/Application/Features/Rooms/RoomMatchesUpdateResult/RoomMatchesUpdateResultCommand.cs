using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Hubs;
using BeatSportsAPI.Domain.Entities.Room;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatchesUpdateResult;
public class RoomMatchesUpdateResultCommand : IRequest<BeatSportsResponse>
{
    public Guid RoomMatchId { get; set; }
    public Guid CustomerId { get; set; }
    public string? Team { get; set; }
}

public class RoomMatchesUpdateResultCommandHandler : IRequestHandler<RoomMatchesUpdateResultCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IHubContext<RoomRequestHub> _hubContext;
    private readonly IEmailService _emailService;

    public RoomMatchesUpdateResultCommandHandler(IBeatSportsDbContext dbContext, IHubContext<RoomRequestHub> hubContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _emailService = emailService;
    }

    public async Task<BeatSportsResponse> Handle(RoomMatchesUpdateResultCommand request, CancellationToken cancellationToken)
    {
        var roomMemberExist = await _dbContext.RoomMembers
                    .Where(rm => rm.CustomerId == request.CustomerId 
                                && rm.RoomMatchId == request.RoomMatchId)
                    .SingleOrDefaultAsync();
        if(roomMemberExist == null)
        {
            throw new NotFoundException("Đã có lỗi, thành viên hoặc phòng đấu không tồn tại.");
        }
        if (string.IsNullOrEmpty(request.Team))
        {
            throw new BadRequestException("Vui lòng chọn đội chiến thắng.");
        }
        roomMemberExist.MatchingResultStatus = (request.Team == "A" ? "VotedTeamA" : "VotedTeamB");
        _dbContext.RoomMembers.Update(roomMemberExist);
        await _dbContext.SaveChangesAsync();
        return new BeatSportsResponse
        {
            Message = "Vote recorded successfully.",
        };
    }
}