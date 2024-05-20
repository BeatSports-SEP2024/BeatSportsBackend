using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;

namespace BeatSportsAPI.Application.Features.Sports.Queries;
public class GetSportCategoryByIdHandler : IRequestHandler<GetSportCategoryByIdCommand, SportCategoriesResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public GetSportCategoryByIdHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public Task<SportCategoriesResponse> Handle(GetSportCategoryByIdCommand request, CancellationToken cancellationToken)
    {
        var isExistedItem = _beatSportsDbContext.SportsCategories
            .Where(sc => sc.Id == request.SportCategoryId && !sc.IsDelete)
            .ProjectTo<SportCategoriesResponse>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
        if(isExistedItem == null)
        {
            throw new BadRequestException($"Do not find sport category with sport category ID: {request.SportCategoryId}");
        }
        return Task.FromResult(isExistedItem);
    }
}