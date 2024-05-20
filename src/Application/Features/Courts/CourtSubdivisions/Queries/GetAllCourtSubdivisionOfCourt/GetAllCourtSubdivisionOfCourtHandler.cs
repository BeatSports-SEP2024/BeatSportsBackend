using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionOfCourt;
public class GetAllCourtSubdivisionOfCourtHandler : IRequestHandler<GetAllCourtSubdivisionOfCourtQuery, PaginatedList<CourtSubdivisionResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllCourtSubdivisionOfCourtHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CourtSubdivisionResponse>> Handle(GetAllCourtSubdivisionOfCourtQuery request, CancellationToken cancellationToken)
    {
        var listCourtSubdivision = await _dbContext.CourtSubdivisions.Where(x => x.CourtId == request.CourtId && !x.IsDelete)
            .ProjectTo<CourtSubdivisionResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);
        return listCourtSubdivision;
    }
}
