using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Rooms.RoomRequests.Queries.GetRoomRequestByCustomer;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetAllRoomMatches;
public class GetAllRoomMatchesHandler : IRequestHandler<GetAllRoomMatchesCommand, RoomRequestsResponseForGetAll>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllRoomMatchesHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public async Task<RoomRequestsResponseForGetAll> Handle(GetAllRoomMatchesCommand request, CancellationToken cancellationToken)
    {
        var query = await _beatSportsDbContext.RoomRequests
            .Include(rr => rr.Customer)
            .Include(rr => rr.RoomMatch)
            .ToListAsync(cancellationToken);

        #region Pending Room Request
        var roomRequests = query.Where(rr => rr.CustomerId == request.CustomerId && rr.JoinStatus == 0);

        if (query == null || !query.Any())
        {
            throw new NotFoundException("Customer Id does not have any room request");
        }

        var roomMatchIds = query.Select(rr => rr.RoomMatchId).ToList();

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

        var pendingResult = roomRequests.Select(c =>
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
                CourtName = c.RoomMatch.Booking.CourtSubdivision.Court.CourtName,
                RoomName = c.RoomMatch.RoomName,
                LevelId = roomMatch?.LevelId ?? default(Guid),
                DatePlaying = roomMatch?.StartTimeRoom ?? DateTime.MinValue,
                DateRequest = c.DateRequest,
                StartTimePlaying = c.RoomMatch.Booking.StartTimePlaying,
                EndTimePlaying = c.RoomMatch.Booking.EndTimePlaying,
                LevelName = roomMatch?.Level?.LevelName ?? "Unknown Level",
                Price = roomMatch?.Booking?.CourtSubdivision?.BasePrice ?? default(decimal),
                NumberOfMember = roomMatch?.RoomMembers?.Count() ?? 0,
                MaxMember = c.RoomMatch.MaximumMember,
                RoomRequestId = c.Id,
            };
        }).ToList();

        #endregion
        #region Join List
        // Danh sách phòng đang tham gia (JoiningStatus = 1 và có data trong table roomMember)
        var joinedRoomRequests = query.Where(rr => rr.Customer.Id == request.CustomerId && (int)rr.JoinStatus == 1);

        var joinedRoomMatchIds = joinedRoomRequests.Select(rr => rr.RoomMatchId).ToList();

        var roomMatchesForJoined = await _beatSportsDbContext.RoomMatches
            .Where(rm => joinedRoomMatchIds.Contains(rm.Id))
            .Include(rm => rm.Booking)
                .ThenInclude(b => b.CourtSubdivision)
                    .ThenInclude(cs => cs.Court)
            .Include(rm => rm.Level)
            .ToListAsync(cancellationToken);

        var roomMembersForJoined = await _beatSportsDbContext.RoomMembers
            .Where(rm => joinedRoomMatchIds.Contains(rm.RoomMatchId))
            .Include(rm => rm.Customer)
                .ThenInclude(c => c.Account)
            .ToListAsync(cancellationToken);

        var joinedResult = joinedRoomRequests.Select(c =>
        {
            var roomMatch = roomMatchesForJoined.FirstOrDefault(rm => rm.Id == c.RoomMatchId);
            var masterMember = roomMembersForJoined.FirstOrDefault(rm => rm.RoomMatchId == c.RoomMatchId && rm.RoleInRoom == 0);
            var masterName = masterMember != null
                ? $"{masterMember.Customer?.Account?.FirstName} {masterMember.Customer?.Account?.LastName}"
                : "Unknown Master Name";

            return new RoomRequestResponseForCustomer
            {
                MasterName = masterName,
                Address = roomMatch?.Booking?.CourtSubdivision?.Court?.Address ?? "Unknown Address",
                CourtName = c.RoomMatch.Booking.CourtSubdivision.Court.CourtName,
                RoomName = c.RoomMatch.RoomName,
                LevelId = roomMatch?.LevelId ?? default(Guid),
                DatePlaying = roomMatch?.StartTimeRoom ?? DateTime.MinValue,
                StartTimePlaying = c.RoomMatch.Booking.StartTimePlaying,
                EndTimePlaying = c.RoomMatch.Booking.EndTimePlaying,
                DateRequest = c.DateRequest,
                LevelName = roomMatch?.Level?.LevelName ?? "Unknown Level",
                Price = roomMatch?.Booking?.CourtSubdivision?.BasePrice ?? default(decimal),
                NumberOfMember = roomMembersForJoined.Count(rm => rm.RoomMatchId == c.RoomMatchId),
                MaxMember = roomMatch?.MaximumMember ?? 0,
                RoomRequestId = c.Id,
            };
        }).ToList();
        #endregion
        #region Public Room
        // Lấy danh sách phòng đang public (IsPrivate = false)
        var welcomeRoomMatches = await _beatSportsDbContext.RoomMatches
            .Where(rm => rm.IsPrivate == false)
            .Include(rm => rm.Booking)
                .ThenInclude(b => b.CourtSubdivision)
                    .ThenInclude(cs => cs.Court)
            .Include(rm => rm.Level)
            .Include(rm => rm.RoomMembers)
                .ThenInclude(rm => rm.Customer)
                    .ThenInclude(c => c.Account)
            .ToListAsync(cancellationToken);

        var welcomeResult = welcomeRoomMatches
            .GroupBy(rm => rm.Id)
            .Select(group => {
                var firstRoomMatch = group.First(); 
                var masterMember = group.SelectMany(rm => rm.RoomMembers).FirstOrDefault(rm => rm.RoleInRoom == 0); 

                return new PublicRoomResponse
                {
                    MasterName = masterMember != null ? $"{masterMember.Customer.Account.FirstName} {masterMember.Customer.Account.LastName}" : "Unknown Master Name",
                    Address = firstRoomMatch.Booking?.CourtSubdivision?.Court?.Address ?? "Unknown Address",
                    LevelId = firstRoomMatch.LevelId,
                    CourtName = firstRoomMatch.Booking.CourtSubdivision.Court.CourtName,
                    RoomName = firstRoomMatch.RoomName,
                    DatePlaying = firstRoomMatch.StartTimeRoom,
                    StartTimePlaying = firstRoomMatch.Booking.StartTimePlaying,
                    EndTimePlaying = firstRoomMatch.Booking.EndTimePlaying,
                    LevelName = firstRoomMatch.Level?.LevelName ?? "Unknown Level",
                    Price = firstRoomMatch.Booking?.CourtSubdivision?.BasePrice ?? default(decimal),
                    NumberOfMember = group.SelectMany(rm => rm.RoomMembers).Count(), 
                    MaxMember = firstRoomMatch.MaximumMember,
                    RoomMatchId = firstRoomMatch.Id
                };
            })
            .ToList();
        #endregion
        return new RoomRequestsResponseForGetAll
        {
            PendingRoomList = pendingResult,
            JoinedRoomList = joinedResult,
            PublicRoomList = welcomeResult
        };
    }
}
