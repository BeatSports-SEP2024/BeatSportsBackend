using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
public class GetAllCourtHandler : IRequestHandler<GetAllCourtCommand, PaginatedList<CourtResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllCourtHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<CourtResponse>> Handle(GetAllCourtCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot be less than 0");
        }

        // Ensure the sport category name is valid and converted to a string only once
        string sportCategoryName = request.SportCategoriesEnums.ToString();

        if (string.IsNullOrEmpty(sportCategoryName))
        {
            throw new BadRequestException("Sport category name cannot be empty");
        }

        IQueryable<Court> query = _dbContext.Courts
            .Include(c => c.CourtCategories)
                .ThenInclude(cc => cc.SportCategory)
                    .ThenInclude(ccc => ccc.CourtSportCategories)
            .Include(c => c.CourtSubdivision)
            .Where(c => c.CourtCategories.Any(cc => cc.SportCategory.Name.Equals(sportCategoryName)));

        var listCourt = query
            .ProjectTo<CourtResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);

        return listCourt;
    }
}
