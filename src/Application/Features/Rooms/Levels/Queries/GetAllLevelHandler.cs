using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.Levels.Queries;
public class GetAllLevelHandler : IRequestHandler<GetAllLevelCommand, PaginatedList<LevelResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllLevelHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<LevelResponse>> Handle(GetAllLevelCommand request, CancellationToken cancellationToken)
    {
        var level = _beatSportsDbContext.Levels
            .Where(lv => !lv.IsDelete)
            .ProjectTo<LevelResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);
        return level;
    }
}
