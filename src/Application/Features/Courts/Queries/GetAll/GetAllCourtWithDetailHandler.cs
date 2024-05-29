using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
public class GetAllCourtWithDetailHandler : IRequestHandler<GetAllCourtWithDetailCommand, PaginatedList<CourtWithDetailResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllCourtWithDetailHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<CourtWithDetailResponse>> Handle(GetAllCourtWithDetailCommand request, CancellationToken cancellationToken)
    {
        // Ensure the sport category name is valid and converted to a string only once
        string sportCategoryName = request.SportCategoriesEnums.ToString();

        IQueryable<Court> query = _beatSportsDbContext.Courts
            .Include(c => c.CourtCategories)
                .ThenInclude(cc => cc.SportCategory)
                    .ThenInclude(ccc => ccc.CourtSportCategories)
            .Include(c => c.CourtSubdivision)
            .Where(c => c.CourtCategories.Any(cc => cc.SportCategory.Name.Equals(sportCategoryName)));

        var listCourt = query.Select(c => new CourtWithDetailResponse
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
