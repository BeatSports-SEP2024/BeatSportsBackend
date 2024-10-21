using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Queries.GetRoomRequestByCustomer;
public class GetRoomRequestByCustomerHandler : IRequestHandler<GetRoomRequestByCustomerCommand, List<RoomRequestResponseForCustomer>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetRoomRequestByCustomerHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<List<RoomRequestResponseForCustomer>> Handle(GetRoomRequestByCustomerCommand request, CancellationToken cancellationToken)
    {
        var roomRequests = await _beatSportsDbContext.RoomRequests
            .Where(rr => rr.CustomerId == request.CustomerId && rr.JoinStatus == 0)
            .Include(rr => rr.Customer)
            .Include(rr => rr.RoomMatch)
            .ToListAsync(cancellationToken);

        if (roomRequests == null || !roomRequests.Any())
        {
            throw new NotFoundException("Customer Id does not have any room request");
        }

        var roomMatchIds = roomRequests.Select(rr => rr.RoomMatchId).ToList();

        var roomMatches = await _beatSportsDbContext.RoomMatches
            .Where(rm => roomMatchIds.Contains(rm.Id))
            .Include(rm => rm.Booking)
                .ThenInclude(b => b.CourtSubdivision)
                    .ThenInclude(cs => cs.Court)
            .Include(rm => rm.Level)
            .ToListAsync(cancellationToken);

        var roomMembers = await _beatSportsDbContext.RoomMembers
            .Where(rm => roomMatchIds.Contains(rm.RoomMatchId))
            .Include(rm => rm.Customer)
                .ThenInclude(c => c.Account)
            .ToListAsync(cancellationToken);

        var result = roomRequests.Select(c =>
        {
            var roomMatch = roomMatches.FirstOrDefault(rm => rm.Id == c.RoomMatchId);

            var masterMember = roomMembers.FirstOrDefault(rm => rm.RoomMatchId == c.RoomMatchId && rm.RoleInRoom == 0);
            var masterName = masterMember != null
                ? $"{masterMember.Customer?.Account?.FirstName} {masterMember.Customer?.Account?.LastName}"
                : "Unknown Master Name";

            return new RoomRequestResponseForCustomer
            {
                MasterName = masterName,
                Address = roomMatch?.Booking?.CourtSubdivision?.Court?.Address ?? "Unknown Address",
                LevelId = roomMatch?.LevelId ?? default(Guid),
                DatePlaying = roomMatch?.StartTimeRoom ?? DateTime.MinValue,
                DateRequest = c.DateRequest,
                LevelName = roomMatch?.Level?.LevelName ?? "Unknown Level",
                Price = roomMatch?.Booking?.CourtSubdivision?.BasePrice ?? default(decimal),
                NumberOfMember = roomMatch?.RoomMembers?.Count() ?? 0,
                MaxMember = c.RoomMatch.MaximumMember,
                RoomRequestId = c.Id,
            };
        }).ToList();

        return result;
    }
}