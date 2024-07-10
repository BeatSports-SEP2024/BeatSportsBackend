using AutoFilterer.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        IQueryable<Court> query = _dbContext.Courts
            .Where(x => !x.IsDelete)
            .OrderByDescending(b => b.Created)
            .Include(x => x.CourtSubdivision);

        var list = query.Select(c => new CourtResponse
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
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize);

        return list;
    }
}
