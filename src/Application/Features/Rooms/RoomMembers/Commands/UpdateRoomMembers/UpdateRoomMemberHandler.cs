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
        var isRoomMemberExisted = _beatSportsDbContext.RoomMembers
            .Where(rm => rm.Id == request.RoomMemberId)
            .SingleOrDefault();

        if (isRoomMemberExisted == null)
        {
            throw new NotFoundException($"{request.RoomMemberId} does not found");
        }
        else
        {
            isRoomMemberExisted.CustomerId = request.CustomerId;
            isRoomMemberExisted.RoomMatchId = request.RoomMatchId;
            isRoomMemberExisted.RoleInRoom = request.RoleInRoom.ToString();

            _beatSportsDbContext.RoomMembers.Update(isRoomMemberExisted);
            await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        }

        return new BeatSportsResponse
        {
            Message = "Update Successfully"
        };
    }
}
