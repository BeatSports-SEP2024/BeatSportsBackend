using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Queries;
public class GetCourtSportCategoryHandler : IRequestHandler<GetCourtSportCategoryCommand, PaginatedList<CourtSportCategoryResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public GetCourtSportCategoryHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<CourtSportCategoryResponse>> Handle(GetCourtSportCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }
        var response = await _beatSportsDbContext.CourtSportCategories
            .ProjectTo<CourtSportCategoryResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);
        return response;
    }
}