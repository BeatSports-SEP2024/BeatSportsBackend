using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaigns;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetCampaignById;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.Room;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetAllRoomMatches;
public class GetAllRoomMatchesHandler : IRequestHandler<GetAllRoomMatchesCommand, PaginatedList<RoomMatchesResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllRoomMatchesHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<PaginatedList<RoomMatchesResponse>> Handle(GetAllRoomMatchesCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        IQueryable<RoomMatch> query = _dbContext.RoomMatches
            .Where(x => !x.IsDelete);

        var list = query.Select(c => new RoomMatchesResponse
        {
            RoomMatchId = c.Id,
            CourtId = c.CourtId,
            LevelId = c.LevelId,
            StartTimeRoom = c.StartTimeRoom,
            EndTimeRoom = c.EndTimeRoom,
            MaximumMember = c.MaximumMember,
            RuleRoom = c.RuleRoom,
            Note = c.Note
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize);

        return list;
    }
}
