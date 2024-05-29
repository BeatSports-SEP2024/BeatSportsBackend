using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.DeleteRoomMembers;
public class DeleteRoomMemberHandler : IRequestHandler<DeleteRoomMemberCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public DeleteRoomMemberHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(DeleteRoomMemberCommand request, CancellationToken cancellationToken)
    {
        var roomMembers = _beatSportsDbContext.RoomMembers
            .Where(rm => rm.Id == request.RoomMemberId)
            .SingleOrDefault();
        if (roomMembers == null)
        {
            throw new NotFoundException($"{request.RoomMemberId} does not existed");
        }
        _beatSportsDbContext.RoomMembers.Remove(roomMembers);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = $"Delete {request.RoomMemberId} successfully"
        };
    }
}
