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
        var isValidRoomMember = _beatSportsDbContext.RoomMembers
            .FirstOrDefault(rm => rm.CustomerId == request.CustomerId 
            && rm.RoomMatchId == request.RoomMatchId);
        if (isValidRoomMember == null)
        {
            throw new NotFoundException($"{request.CustomerId} or {request.RoomMatchId} does not existed");
        }
        _beatSportsDbContext.RoomMembers.Remove(isValidRoomMember);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = $"Delete room member successfully"
        };
    }
}
