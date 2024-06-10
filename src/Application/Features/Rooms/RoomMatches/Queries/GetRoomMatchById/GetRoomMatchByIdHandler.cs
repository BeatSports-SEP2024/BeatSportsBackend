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

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetRoomMatchById;
public class GetRoomMatchByIdHandler : IRequestHandler<GetRoomMatchByIdCommand, RoomMatchesResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetRoomMatchByIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<RoomMatchesResponse> Handle(GetRoomMatchByIdCommand request, CancellationToken cancellationToken)
    {
        IQueryable<RoomMatch> query = _dbContext.RoomMatches
            .Where(x => x.Id == request.RoomMatchId && !x.IsDelete);

        var room = query.Select(c => new RoomMatchesResponse
        {
            RoomMatchId = c.Id,
            BookingId = c.BookingId,
            LevelId = c.LevelId,
            StartTimeRoom = c.StartTimeRoom,
            EndTimeRoom = c.EndTimeRoom,
            MaximumMember = c.MaximumMember,
            RuleRoom = c.RuleRoom,
            Note = c.Note
        }).SingleOrDefault();

        if (room == null)
        {
            throw new NotFoundException($"Do not find RoomMatch with RoomMatch ID: {request.RoomMatchId}");
        }
        return Task.FromResult(room);
    }
}
