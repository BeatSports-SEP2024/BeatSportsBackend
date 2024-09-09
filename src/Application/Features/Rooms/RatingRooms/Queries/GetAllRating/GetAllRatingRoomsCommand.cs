using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RatingRooms.Queries.GetAllRating;
public class GetAllRatingRoomsCommand : IRequest<List<RatingRoomsResponse>>
{
}

public class GetAllRatingRoomsCommandHandler : IRequestHandler<GetAllRatingRoomsCommand, List<RatingRoomsResponse>>
{
    private readonly IMapper _mapper;
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetAllRatingRoomsCommandHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _mapper = mapper;
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<List<RatingRoomsResponse>> Handle(GetAllRatingRoomsCommand request, CancellationToken cancellationToken)
    {
        var response = await _beatSportsDbContext.RatingRooms.Where(r => !r.IsDelete).ToListAsync();
        var data = _mapper.Map<List<RatingRoomsResponse>>(response);
        return data;
    }
}
