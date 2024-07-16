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
public class GetAllCourtSubdivisionOfCourtHandler : IRequestHandler<GetAllCourtSubdivisionOfCourtQuery, PaginatedList<CourtSubdivisionResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllCourtSubdivisionOfCourtHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<CourtSubdivisionResponse>> Handle(GetAllCourtSubdivisionOfCourtQuery request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size must be greater than 0");
        }

        var courtsQuery = _dbContext.Courts
            .Where(c => !c.IsDelete && c.Id == request.CourtId);

        var subdivisionsQuery = courtsQuery.SelectMany(c => c.CourtSubdivision.Select(cs => new CourtSubdivisionResponse
        {
            Id = cs.Id,
            CourtId = c.Id,
            CourtName = c.CourtName,
            Description = cs.CourtSubdivisionDescription, 
            ImageURL = ImageUrlSplitter.SplitAndGetFirstImageUrls(cs.Court.ImageUrls),  
            IsActive = cs.IsActive,
            BasePrice = cs.BasePrice,
            CourtSubdivisionName = cs.CourtSubdivisionName 
        })).PaginatedListAsync(request.PageIndex, request.PageSize);

        return subdivisionsQuery;
    }
}
