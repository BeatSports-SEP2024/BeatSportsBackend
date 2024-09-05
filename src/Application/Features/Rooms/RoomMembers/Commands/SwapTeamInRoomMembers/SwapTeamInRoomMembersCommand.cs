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
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.SwapTeamInRoomMembers;
public class SwapTeamInRoomMembersCommand : IRequest<BeatSportsResponse>
{
    public Guid RoomMatchId { get; set; }
    public Guid CustomerId { get; set; }
    public string? Team { get; set; }
}

public class SwapTeamInRoomMembersCommandHandler : IRequestHandler<SwapTeamInRoomMembersCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IHubContext<RoomRequestHub> _hubContext;

    public SwapTeamInRoomMembersCommandHandler(IBeatSportsDbContext beatSportsDbContext, IHubContext<RoomRequestHub> hubContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _hubContext = hubContext;
    }

    public async Task<BeatSportsResponse> Handle(SwapTeamInRoomMembersCommand request, CancellationToken cancellationToken)
    {
        var roomMatch = await _beatSportsDbContext.RoomMatches
            .Where(rm => rm.Id == request.RoomMatchId)
            .FirstOrDefaultAsync();
        if (roomMatch == null)
        {
            throw new NotFoundException("Không tìm thấy phòng này!!");
        }
        var teamMemberCount = roomMatch.MaximumMember / 2;

        var roomMemberTeamA = _beatSportsDbContext.RoomMembers
                    .Where(rm => rm.RoomMatchId == request.RoomMatchId && rm.Team == "A")
                    .ToList()
                    .Count();
        var roomMemberTeamB = _beatSportsDbContext.RoomMembers
            .Where(rm => rm.RoomMatchId == request.RoomMatchId && rm.Team == "B")
            .ToList()
            .Count();

        var roomMemberTeamCurrent = await _beatSportsDbContext.RoomMembers
            .Where(rm => rm.RoomMatchId == request.RoomMatchId && rm.CustomerId == request.CustomerId)
            .SingleOrDefaultAsync();
        if(roomMemberTeamCurrent!.Team == request.Team)
        {
            throw new BadRequestException("Đã có lỗi xảy ra khi đổi đội chơi");
        }

        // Nếu muốn đổi sang Team A
        if (request.Team == "A")
        {
            // Kiểm tra xem Team A đã đầy hay chưa
            if (roomMemberTeamA < teamMemberCount)
            {
                // Đổi sang Team A
                roomMemberTeamCurrent.Team = "A";
                _beatSportsDbContext.RoomMembers.Update(roomMemberTeamCurrent);
            }
            else
            {
                throw new BadRequestException("Team A đã đầy, không thể chuyển sang đội này.");
            }
        }
        // Nếu muốn đổi sang Team B
        else if (request.Team == "B")
        {
            // Kiểm tra xem Team B đã đầy hay chưa
            if (roomMemberTeamB < teamMemberCount)
            {
                // Đổi sang Team B
                roomMemberTeamCurrent.Team = "B";
                _beatSportsDbContext.RoomMembers.Update(roomMemberTeamCurrent);
            }
            else
            {
                throw new BadRequestException("Team B đã đầy, không thể chuyển sang đội này.");
            }
        }
        else
        {
            throw new BadRequestException("Đội không hợp lệ.");
        }

        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        await _hubContext.Clients.Group(roomMatch.Id.ToString()).SendAsync("UpdateRoom", "SwapTeam", request.CustomerId);

        return new BeatSportsResponse { Message = "Chuyển đội thành công." };
    }
}
