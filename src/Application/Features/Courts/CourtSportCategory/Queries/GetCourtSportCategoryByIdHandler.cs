using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Queries;
public class GetCourtSportCategoryByIdHandler : IRequestHandler<GetCourtSportCategoryByIdCommand, CourtSportCategoryResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public GetCourtSportCategoryByIdHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public Task<CourtSportCategoryResponse> Handle(GetCourtSportCategoryByIdCommand request, CancellationToken cancellationToken)
    {
        var isExistedItem = _beatSportsDbContext.CourtSportCategories
            .Where(csc => csc.Id == request.CourtSportCategoryId)
            .ProjectTo<CourtSportCategoryResponse>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
        if (isExistedItem == null)
        {
            throw new NotFoundException("This court sport category does not existed");
        }
        return Task.FromResult(isExistedItem);
    }
}
