using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Features.Courts.Queries.GetById;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAllCourtsByOwnerId;
public class GetAllCourtsByOwnerIdHandler : IRequestHandler<GetAllCourtsByOwnerIdCommand, PaginatedList<CourtResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllCourtsByOwnerIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<PaginatedList<CourtResponse>> Handle(GetAllCourtsByOwnerIdCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        IQueryable<Court> query = _dbContext.Courts
            .Where(x => !x.IsDelete && x.OwnerId == request.OwnerId)
            .Include(x => x.CourtSubdivision);

        var list = query.Select(c => new CourtResponse
        {
            Id = c.Id,
            OwnerName = c.Owner.Account.FirstName + " " + c.Owner.Account.LastName,
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
