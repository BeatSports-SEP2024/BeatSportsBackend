using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetRoomMatchById;
public class GetRoomMatchByIdHandler : IRequestHandler<GetRoomMatchByIdCommand, RoomMatchesDetailResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetRoomMatchByIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<RoomMatchesDetailResponse> Handle(GetRoomMatchByIdCommand request, CancellationToken cancellationToken)
    {
        var query = _dbContext.RoomMatches
            .Where(x => x.Id == request.RoomMatchId && !x.IsDelete)
            .Include(x => x.Level)
            .Include(x => x.RatingRoom)
            .Include(x => x.RoomMembers)
                .ThenInclude(rm => rm.Customer)
                    .ThenInclude(c => c.Account)
            .Include(x => x.RoomRequests)
            .Include(x => x.Booking)
                .ThenInclude(c => c.CourtSubdivision)
            .FirstOrDefault();

        if (query == null)
        {
            throw new NotFoundException();
        }

        var court = _dbContext.Courts
                    .Where(x => x.Id == query.Booking.CourtSubdivision.CourtId)
                    .FirstOrDefault();

        var courtImgList = court?.ImageUrls.Split(",") ?? Array.Empty<string>();

        var customer = _dbContext.Customers
                    .Where(x => x.Id == request.CustomerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

        var roomRequests = new List<RoomRequestInRoom>();
        var roomMembers = new List<RoomMemberInRoomResponse>();

        var masterMember = query.RoomMembers
            .Where(x => x.RoleInRoom == Domain.Enums.RoleInRoomEnums.Master)
            .Select(rm => new
            {
                rm.Customer.Account.FirstName,
                rm.Customer.Account.LastName,
                rm.Customer.Account.ProfilePictureURL
            })
            .FirstOrDefault();

        if (query.RoomMembers.Any(x => x.RoleInRoom == Domain.Enums.RoleInRoomEnums.Master
                                    && x.CustomerId == request.CustomerId))
        {
            foreach (var roomRequest in query.RoomRequests)
            {
                if (roomRequest.JoinStatus == Domain.Enums.RoomRequestEnums.Accepted || roomRequest.JoinStatus == Domain.Enums.RoomRequestEnums.Declined)
                {
                    continue;
                }
                var cus = _dbContext.Customers
                    .Where(x => x.Id == roomRequest.CustomerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

                var result = new RoomRequestInRoom()
                {
                    CustomerId = cus.Id,
                    CustomerAvatar = cus.Account.ProfilePictureURL,
                    CustomerName = cus.Account.FirstName + " " + cus.Account.LastName,
                    RoomRequestsId = roomRequest.Id,
                    JoinStatus = roomRequest.JoinStatus,
                };

                roomRequests.Add(result);
            }
        }

        foreach (var roomMember in query.RoomMembers)
        {
            var cus = _dbContext.Customers
                    .Where(x => x.Id == roomMember.CustomerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

            var result = new RoomMemberInRoomResponse()
            {
                CustomerId = cus.Id,
                CustomerImage = cus.Account.ProfilePictureURL,
                CustomerName = cus.Account.FirstName + " " + cus.Account.LastName,
                RoleInRoom = roomMember.RoleInRoom.GetDescriptionFromEnum(),
                Team = roomMember.Team,
                MatchingResultStatus = roomMember.MatchingResultStatus,
            };

            roomMembers.Add(result);
        }

        var findingStatusOfRoom = _dbContext.RoomRequests
            .Where(rq => rq.CustomerId == request.CustomerId && rq.RoomMatchId == request.RoomMatchId)
            .SingleOrDefault();

        var master = query.RoomMembers.Any(x => x.RoleInRoom == Domain.Enums.RoleInRoomEnums.Master
                                    && x.CustomerId == request.CustomerId);

        var courtSubSetting = _dbContext.CourtSubdivisionSettings.Where(cs => cs.Id == query.Booking.CourtSubdivision.CourtSubdivisionSettingId).SingleOrDefault();
        var sport = _dbContext.SportsCategories.Where(c => c.Id == courtSubSetting!.SportCategoryId).SingleOrDefault();

        // chia tiền
        var teamSize = (decimal)query.MaximumMember / 2;
        var teamCost = (query.Booking.TotalAmount * (decimal)(query.RatingRoom?.WinRatePercent ?? 0));

        var room = new RoomMatchesDetailResponse()
        {
            RoomMatchId = request.RoomMatchId,
            VotesCount = query.VoteCount,
            CourtName = court.CourtName,
            CourtImage = courtImgList,
            RoomName = query.RoomName,
            CustomerImage = customer.Account.ProfilePictureURL,
            CustomerName = customer.Account.FirstName + " " + customer.Account.LastName,
            CourtDescription = court.Description,
            CustomerPhone = customer.Account.PhoneNumber,
            Address = court.Address,
            StartTimePlaying = query.Booking.StartTimePlaying,
            EndTimePlaying = query.Booking.EndTimePlaying,
            PlayingDate = query.Booking.PlayingDate,
            StartTimeRoom = query.StartTimeRoom,
            EndTimeRoom = query.EndTimeRoom,
            CountMember = query.RoomMembers.Count,
            PlayingCosts = (double)(teamCost / teamSize),
            RuleRoom = query.RuleRoom,
            JoiningRequest = roomRequests,
            RoomMembers = roomMembers,
            JoinedIfPendingStatus = (master ? Domain.Enums.RoomRequestEnums.Accepted : (findingStatusOfRoom == null ? Domain.Enums.RoomRequestEnums.Declined : findingStatusOfRoom.JoinStatus)).ToString(),
            IsPrivate = query.IsPrivate,
            MaximumMember = query.MaximumMember,
            Note = query.Note,

            // Thêm thông tin chủ phòng
            MasterName = $"{masterMember.FirstName} {masterMember.LastName}",
            MasterAvatar = masterMember.ProfilePictureURL,

            // phần thêm 
            SportName = sport.Name,
            SportCourtTypeName = courtSubSetting.CourtType,
            RoomMatchTypeName = query.RoomMatchTypeName,
            DescriptionRating = query.RatingRoom?.Description,
            WinRatePercent = query.RatingRoom?.WinRatePercent,
            LoseRatePercent = query.RatingRoom?.LoseRatePercent,
        };

        return Task.FromResult(room);
    }
}
