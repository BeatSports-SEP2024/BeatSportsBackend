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

    public async Task<PaginatedList<CourtSubdivisionResponse>> Handle(GetAllCourtSubdivisionOfCourtQuery request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        IQueryable<CourtSubdivision> query = _dbContext.CourtSubdivisions
            .Where(x => !x.IsDelete)
            .Include(x => x.Court);

        var list = query.Select(c => new CourtSubdivisionResponse
        {
            Id = c.Id,
            CourtId = c.CourtId,
            CourtName = c.Court.CourtName,
            //Description = c.Description,
            //ImageURL = c.ImageURL,
            IsActive = c.IsActive,
            BasePrice = c.BasePrice,
            CourtSubdivisionName = c.CourtSubdivisionName
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize);

        return await list;
    }
}
