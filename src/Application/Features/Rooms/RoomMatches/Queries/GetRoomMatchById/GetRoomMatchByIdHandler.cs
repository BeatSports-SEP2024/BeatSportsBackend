using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignById;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.Room;
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
            .Include(x => x.RoomMembers)
            .Include(x => x.RoomRequests)
            .Include(x => x.Booking).ThenInclude(c => c.CourtSubdivision).FirstOrDefault();
            
        var court = _dbContext.Courts
                    .Where(x => x.Id == query.Booking.CourtSubdivision.CourtId).FirstOrDefault();

        var courtImgList = court.ImageUrls.Split(",");

        var customer = _dbContext.Customers
                    .Where(x => x.Id == request.CustomerId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

        var roomRequests = new List<RoomRequestInRoom>();
        var roomMembers = new List<RoomMemberInRoomResponse>();

        if (query.RoomMembers.Any(x => x.RoleInRoom == Domain.Enums.RoleInRoomEnums.Master 
                                    && x.CustomerId == request.CustomerId))
        {
            foreach (var roomRequest in query.RoomRequests)
            {
                var cus = _dbContext.Customers
                        .Where(x => x.Id == roomRequest.CustomerId)
                        .Include(x => x.Account)
                        .FirstOrDefault();

                var result = new RoomRequestInRoom()
                {
                    AccountId = cus.Account.Id,
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
                AccountId = cus.Account.Id,
                CustomerImage = cus.Account.ProfilePictureURL,
                CustomerName = cus.Account.FirstName + " " + cus.Account.LastName,
                RoleInRoom = roomMember.RoleInRoom
            };

            roomMembers.Add(result);
        }

        var room = new RoomMatchesDetailResponse()
        {
            RoomMatchId = request.RoomMatchId,
            CourtName = court.CourtName,
            CourtImage = courtImgList,
            RoomName = query.RoomName,
            CustomerImage = customer.Account.ProfilePictureURL,
            CustomerName = customer.Account.FirstName + " " + customer.Account.LastName,
            CustomerPhone = customer.Account.PhoneNumber,
            Address = court.Address,
            StartTimePlaying = query.Booking.StartTimePlaying,
            EndTimePlaying = query.Booking.EndTimePlaying,
            StartTimeRoom = query.StartTimeRoom, 
            EndTimeRoom = query.EndTimeRoom,
            CountMember = query.RoomMembers.Count,
            PlayingCosts = (int)query.Booking.TotalAmount/ query.MaximumMember,
            RuleRoom = query.RuleRoom,
            JoiningRequest = roomRequests,
            RoomMembers = roomMembers,
            IsPrivate = query.IsPrivate,
            MaximumMember = query.MaximumMember,
        };

        if (room == null)
        {
            throw new NotFoundException($"Do not find RoomMatch with RoomMatch ID: {request.RoomMatchId}");
        }

        return Task.FromResult(room);
    }
}
