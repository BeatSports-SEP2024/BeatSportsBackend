using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMembers.Commands.CreateRoomMembers;
public class CreateRoomMemberHandler : IRequestHandler<CreateRoomMemberCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public CreateRoomMemberHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateRoomMemberCommand request, CancellationToken cancellationToken)
    {
        var isCustomerExisted = _beatSportsDbContext.Customers
            .Where(c => c.Id == request.CustomerId && !c.IsDelete)
            .SingleOrDefault();

        var isRoomMatchExisted = _beatSportsDbContext.RoomMatches
            .Where(rm => rm.Id == request.RoomMatchId && !rm.IsDelete)
            .SingleOrDefault();

        if (isCustomerExisted == null || isRoomMatchExisted == null)
        {
            throw new NotFoundException($"{request.RoomMatchId} or {request.CustomerId} does not existed");
        }

        var newRoomMember = new RoomMember
        {
            CustomerId = request.CustomerId,
            RoomMatchId = request.RoomMatchId,
            RoleInRoom = RoleInRoomEnums.Member,
        };
        _beatSportsDbContext.RoomMembers.Add(newRoomMember);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = "Add new players successfully"
        };
    }
}
