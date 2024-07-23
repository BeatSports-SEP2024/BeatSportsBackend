using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.UpdateRoomMembers;
public class UpdateRoomMemberHandler : IRequestHandler<UpdateRoomMemberCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public UpdateRoomMemberHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(UpdateRoomMemberCommand request, CancellationToken cancellationToken)
    {
        var isValidRoomMember = _beatSportsDbContext.RoomMembers
             .FirstOrDefault(rm => rm.CustomerId == request.CustomerId
             && rm.RoomMatchId == request.RoomMatchId);

        if (isValidRoomMember == null)
        {
            throw new NotFoundException($"{request.RoomMemberId} does not found");
        }
        else
        {
            isValidRoomMember.CustomerId = request.CustomerId;
            isValidRoomMember.RoomMatchId = request.RoomMatchId;
            isValidRoomMember.RoleInRoom = request.RoleInRoom;

            _beatSportsDbContext.RoomMembers.Update(isValidRoomMember);
            await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        }

        return new BeatSportsResponse
        {
            Message = "Update Successfully"
        };
    }
}
