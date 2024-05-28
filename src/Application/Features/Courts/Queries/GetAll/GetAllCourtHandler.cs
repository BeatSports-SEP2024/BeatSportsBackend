using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;
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

        IQueryable<Court> query = _dbContext.Courts
            .Include(c => c.CourtCategories)
                .ThenInclude(cc => cc.SportCategory)
                    .ThenInclude(ccc => ccc.CourtSportCategories)
            .Include(c => c.CourtSubdivision)
            .Where(c => c.CourtCategories.Any(cc => cc.SportCategory.Name.Equals(sportCategoryName)));

        var listCourt = query.Select(c => new CourtResponse
            {
                Id = c.Id,
                OwnerId = c.OwnerId,
                Description = c.Description,
                CourtName = c.CourtName,
                Address = c.Address,
                GoogleMapURLs = c.GoogleMapURLs,
                TimeStart = c.TimeStart,
                TimeEnd = c.TimeEnd,
                PlaceId = c.PlaceId,
                BasePrice = c.CourtSubdivision.Select(cs => cs.BasePrice).ToList(),
                SportCategoriesEnums = (List<SportCategoriesEnums>)c.CourtCategories
                    .Select(cc => cc.SportCategory.Name)
                    .Select(name => ParseEnumsExtension.ParseEnum<SportCategoriesEnums>(name))
            }).PaginatedListAsync(request.PageIndex, request.PageSize);
        return listCourt;
    }
}
