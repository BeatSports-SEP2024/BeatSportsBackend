using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetById;
public class GetCourtByIdHandler : IRequestHandler<GetCourtByIdCommand, CourtResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCourtByIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<CourtResponse> Handle(GetCourtByIdCommand request, CancellationToken cancellationToken)
    {
        var court = _dbContext.Courts
            .Where(x => x.Id == request.CourtId && !x.IsDelete)
            .ProjectTo<CourtResponse>(_mapper.ConfigurationProvider).SingleOrDefault();
        if (court == null)
        {
            throw new NotFoundException($"Do not find court with court ID: {request.CourtId}");
        }
        return Task.FromResult(court);
    }
}
