using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionOfCourt;
public class GetAllCourtSubdivisionOfCourtHandler : IRequestHandler<GetAllCourtSubdivisionOfCourtQuery, PaginatedList<CourtSubdivisionResponseV3>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllCourtSubdivisionOfCourtHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CourtSubdivisionResponseV3>> Handle(GetAllCourtSubdivisionOfCourtQuery request, CancellationToken cancellationToken)
    {
        var currentDate = DateTime.Now;

        // Prepare the IQueryable query
        var query = _dbContext.Courts
            .Where(c => !c.IsDelete && c.Id == request.CourtId)
            .SelectMany(c => c.CourtSubdivision)
            .Select(cs => new {
                Subdivision = cs,
                Court = cs.Court,
                TimeCheckings = cs.TimeCheckings
                    .Where(tc => tc.StartTime.Date == currentDate.Date && tc.EndTime <= currentDate)
                    .OrderByDescending(tc => tc.StartTime)
                    .FirstOrDefault()
            })
            .Select(x => new CourtSubdivisionResponseV3
            {
                Id = x.Subdivision.Id,
                CourtId = x.Court.Id,
                CourtName = x.Court.CourtName,
                Description = x.Subdivision.CourtSubdivisionDescription,
                ImageURL = ImageUrlSplitter.SplitAndGetFirstImageUrls(x.Court.ImageUrls),
                IsActive = x.Subdivision.IsActive,
                BasePrice = x.Subdivision.BasePrice,
                CourtSubdivisionName = x.Subdivision.CourtSubdivisionName,
                Status = !x.Subdivision.IsActive ? "Đang bảo trì" :
                    x.TimeCheckings == null ? "Chưa có lịch" :
                    (x.TimeCheckings.StartTime <= currentDate && x.TimeCheckings.EndTime >= currentDate ? "Đang cho thuê" :
                    (x.TimeCheckings.StartTime > currentDate ? "Đã đặt" : "Không có sử dụng"))
            });

        // Now use CreateAsync to paginate results
        var paginatedResult = await PaginatedList<CourtSubdivisionResponseV3>.CreateAsync(query, request.PageIndex, request.PageSize);

        return paginatedResult;
    }
}