using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionById;
public class GetCourtSubdivisionByIdHandler : IRequestHandler<GetCourtSubdivisionByIdQuery, CourtSubdivisionResponse?>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCourtSubdivisionByIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CourtSubdivisionResponse?> Handle(GetCourtSubdivisionByIdQuery request, CancellationToken cancellationToken)
    {
        var courtSubdivision = await _dbContext.CourtSubdivisions
                                               .Where(x => x.Id == request.CourtSubdivisionId && !x.IsDelete)
                                               .ProjectTo<CourtSubdivisionResponse>(_mapper.ConfigurationProvider)
                                               .SingleOrDefaultAsync();
        return courtSubdivision;
    }
}
