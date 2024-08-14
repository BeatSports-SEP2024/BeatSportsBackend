using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Enums;
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
        var response = new RoomRequestsResponseForGetAll
        {
            PendingRoomList = new List<RoomRequestResponseForCustomer>(),
            JoinedRoomList = new List<JoinListResponse>(),
            PublicRoomList = new List<PublicRoomResponse>()
        };

        var query = await _beatSportsDbContext.RoomRequests
            .Include(rr => rr.Customer)
            .Include(rr => rr.RoomMatch)
            .ToListAsync(cancellationToken);

        #region Pending Room Request
        var roomRequests = query.Where(rr => rr.CustomerId == request.CustomerId && rr.JoinStatus == 0);

        if (query == null || !query.Any())
        {
            response.PendingRoomList = new List<RoomRequestResponseForCustomer>();
        }

        var roomMatchIds = query.Select(rr => rr.RoomMatchId).ToList();

        var roomMatches = await _beatSportsDbContext.RoomMatches
            .Where(rm => roomMatchIds.Contains(rm.Id) && (rm.Booking.PlayingDate.Date > DateTime.Now.Date
                                                        || (rm.Booking.PlayingDate.Date == DateTime.Now.Date
                                                            && rm.Booking.StartTimePlaying > DateTime.Now.TimeOfDay)))
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
            if (roomMatch == null) return null;

            var masterMember = roomMembers.FirstOrDefault(rm => rm.RoomMatchId == c.RoomMatchId && rm.RoleInRoom == 0);
            var masterName = masterMember != null
                ? $"{masterMember.Customer.Account.FirstName} {masterMember.Customer.Account.LastName}"
                : "Unknown Master Name";

            return new RoomRequestResponseForCustomer
            {
                MasterName = masterName,
                Address = roomMatch.Booking.CourtSubdivision.Court.Address,
                CourtName = c.RoomMatch.Booking.CourtSubdivision.Court.CourtName,
                RoomName = c.RoomMatch.RoomName,
                LevelId = roomMatch.LevelId,
                DatePlaying = roomMatch.Booking.PlayingDate,
                DateRequest = c.DateRequest,
                StartTimePlaying = c.RoomMatch.Booking.StartTimePlaying,
                EndTimePlaying = c.RoomMatch.Booking.EndTimePlaying,
                LevelName = roomMatch.Level.LevelName,
                Price = roomMatch.Booking.CourtSubdivision.BasePrice,
                NumberOfMember = roomMatch.RoomMembers.Count(),
                MaxMember = c.RoomMatch.MaximumMember,
                RoomRequestId = c.Id,
                RoomMatchId = c.RoomMatch.Id,
            };
        })
        .Where(x => x != null)
        .OrderBy(x => x.DatePlaying)
        .ToList();
        response.PendingRoomList = pendingResult;

        #endregion

        #region Join List 
        // Danh sách phòng đang tham gia (JoiningStatus = 1 và có data trong table roomMember)
        var joinedRoomRequests = query.Where(rr => rr.CustomerId == request.CustomerId && (int)rr.JoinStatus == 1);

        var joinedRoomMatchIds = joinedRoomRequests.Select(rr => rr.RoomMatchId).ToList();

        // Thêm những RoomMatchId từ RoomMembers để lấy những người đã có mặt trong roomMatch ngay từ đầu.
        var initialMembersRoomMatchIds = _beatSportsDbContext.RoomMembers
            .Where(rm => rm.CustomerId == request.CustomerId)
            .Select(rm => rm.RoomMatchId)
            .Distinct()
            .ToList();

        // Gộp danh sách RoomMatchId có JoiningStatus = 1 và những người đã có mặt trong roomMatch.
        var allRoomMatchIds = joinedRoomMatchIds.Concat(initialMembersRoomMatchIds).Distinct().ToList();

        var roomMatchesForJoined = await _beatSportsDbContext.RoomMatches
            .Where(rm => allRoomMatchIds.Contains(rm.Id) && (rm.Booking.PlayingDate.Date > DateTime.Now.Date
                                                            || (rm.Booking.PlayingDate.Date == DateTime.Now.Date
                                                                && rm.Booking.StartTimePlaying > DateTime.Now.TimeOfDay)))
            .Include(rm => rm.Booking)
                .ThenInclude(b => b.CourtSubdivision)
                    .ThenInclude(cs => cs.Court)
            .Include(rm => rm.Level)
            .ToListAsync(cancellationToken);

        var roomMembersForJoined = await _beatSportsDbContext.RoomMembers
            .Where(rm => allRoomMatchIds.Contains(rm.RoomMatchId))
            .Include(rm => rm.Customer)
                .ThenInclude(c => c.Account)
            .ToListAsync(cancellationToken);

        var joinedResult = allRoomMatchIds.Select(id =>
        {
            var roomMatch = roomMatchesForJoined.FirstOrDefault(rm => rm.Id == id);
            if (roomMatch == null) return null;

            var masterMember = roomMembersForJoined.FirstOrDefault(rm => rm.RoomMatchId == id && rm.RoleInRoom == 0);
            var masterName = masterMember != null
                ? $"{masterMember.Customer.Account.FirstName} {masterMember.Customer.Account.LastName}"
                : "Unknown Master Name";

            return new JoinListResponse
            {
                MasterName = masterName,
                Address = roomMatch.Booking.CourtSubdivision.Court.Address,
                CourtName = roomMatch.Booking.CourtSubdivision.Court.CourtName,
                RoomName = roomMatch.RoomName,
                LevelId = roomMatch.LevelId,
                DatePlaying = roomMatch.Booking.PlayingDate,
                StartTimePlaying = roomMatch.Booking.StartTimePlaying,
                EndTimePlaying = roomMatch.Booking.EndTimePlaying,
                LevelName = roomMatch.Level.LevelName,
                Price = roomMatch.Booking.CourtSubdivision.BasePrice,
                NumberOfMember = roomMembersForJoined.Count(rm => rm.RoomMatchId == id),
                MaxMember = roomMatch.MaximumMember,
                RoomMatchId = id,
            };
        })
        .Where(x => x != null)
        .OrderBy(x => x.DatePlaying)
        .ToList();
        response.JoinedRoomList = joinedResult;
        #endregion

        #region Public Room
        // Lấy danh sách phòng đang public (IsPrivate = false)
        var welcomeRoomMatches = await _beatSportsDbContext.RoomMatches
            .Where(rm => !allRoomMatchIds.Contains(rm.Id) && (rm.Booking.PlayingDate.Date > DateTime.Now.Date
                                                            || (rm.Booking.PlayingDate.Date == DateTime.Now.Date
                                                                && rm.Booking.StartTimePlaying > DateTime.Now.TimeOfDay)))
            .Include(rm => rm.Booking)
                .ThenInclude(b => b.CourtSubdivision)
                    .ThenInclude(cs => cs.Court)
            .Include(rm => rm.Level)
            .Include(rm => rm.RoomMembers)
                .ThenInclude(rm => rm.Customer)
                    .ThenInclude(c => c.Account)
            .ToListAsync(cancellationToken);

        string sportFilter = request.sportFilter.ToString() ?? string.Empty;
        if (!string.IsNullOrEmpty(sportFilter))
        {
            switch (sportFilter)
            {
                case "Football":
                    welcomeRoomMatches = welcomeRoomMatches.Where(c => c.SportCategory == 0).ToList();
                    break;

                case "Volleyball":
                    welcomeRoomMatches = welcomeRoomMatches.Where(c => (int)c.SportCategory == 1).ToList();
                    break;

                case "Badminton":
                    welcomeRoomMatches = welcomeRoomMatches.Where(c => (int)c.SportCategory == 2).ToList();
                    break;
            }
        }

        var welcomeResult = welcomeRoomMatches
            .GroupBy(rm => rm.Id)
            .Select(group =>
            {
                var firstRoomMatch = group.First();
                if (firstRoomMatch == null) return null;

                var masterMember = group.SelectMany(rm => rm.RoomMembers).FirstOrDefault(rm => rm.RoleInRoom == 0);
                var masterName = masterMember != null
                    ? $"{masterMember.Customer.Account.FirstName} {masterMember.Customer.Account.LastName}"
                    : "Unknown Master Name";

                return new PublicRoomResponse
                {
                    MasterName = masterName,
                    Address = firstRoomMatch.Booking.CourtSubdivision.Court.Address,
                    LevelId = firstRoomMatch.LevelId,
                    CourtName = firstRoomMatch.Booking.CourtSubdivision.Court.CourtName,
                    RoomName = firstRoomMatch.RoomName,
                    DatePlaying = firstRoomMatch.Booking.PlayingDate,
                    StartTimePlaying = firstRoomMatch.Booking.StartTimePlaying,
                    EndTimePlaying = firstRoomMatch.Booking.EndTimePlaying,
                    LevelName = firstRoomMatch.Level.LevelName,
                    Price = firstRoomMatch.Booking.CourtSubdivision.BasePrice,
                    NumberOfMember = group.SelectMany(rm => rm.RoomMembers).Count(),
                    MaxMember = firstRoomMatch.MaximumMember,
                    RoomMatchId = firstRoomMatch.Id,
                };
            })
            .Where(x => x != null)
            .OrderBy(x => x.DatePlaying)
            .ToList();

        string requestedLevelDescriptions = request.Level?.ToString() ?? string.Empty;

        if (!string.IsNullOrEmpty(requestedLevelDescriptions))
        {
            var requestedLevels = requestedLevelDescriptions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                             .Select(level => level.Trim())
                                                             .ToList();

            var validLevels = new List<string> { "Trung bình", "Tập sự", "Chuyên gia" };

            if (requestedLevels.Any(level => !validLevels.Contains(level)))
            {
                throw new NotFoundException("Your search does not match any record \"Trung bình\", \"Tập sự\", \"Chuyên gia\"");
            }

            welcomeResult = welcomeResult.Where(x => requestedLevels.Contains(x.LevelName)).ToList();
        }

        string queryContains = request.Query?.RemoveDiacritics() ?? string.Empty;

        if (!string.IsNullOrEmpty(queryContains))
        {
            welcomeResult = welcomeResult.Where(x => x.MasterName.RemoveDiacritics().Contains(queryContains, StringComparison.OrdinalIgnoreCase)
                                             || x.Address.RemoveDiacritics().Contains(queryContains, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        response.PublicRoomList = welcomeResult;

        #endregion
        return response;
    }
}