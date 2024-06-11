using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAllCourtsByOwnerId;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.MapBox;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetListCourtsNearBy;
public class GetListCourtsNearByHandler : IRequestHandler<GetListCourtsNearByCommand, List<CourtResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetListCourtsNearByHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<List<CourtResponse>> Handle(GetListCourtsNearByCommand request, CancellationToken cancellationToken)
    {
        var distanceCal = new DistanceCalculation();

        IQueryable<Court> query = _dbContext.Courts
            .Where(x => !x.IsDelete)
            .Include(x => x.CourtSubdivision);
            
        //Goi service tinh khoang cach
        var result = distanceCal.GetDistancesAsync(Double.Parse(request.Latitude), Double.Parse(request.Longitude), query);

        var list = result.Result.Select(c => new CourtResponse
        {
            Id = c.Location.Id,
            OwnerId = c.Location.OwnerId,
            Description = c.Location.Description,
            CourtName = c.Location.CourtName,
            Address = c.Location.Address,
            GoogleMapURLs = c.Location.GoogleMapURLs,
            TimeStart = c.Location.TimeStart,
            TimeEnd = c.Location.TimeEnd,
            PlaceId = c.Location.PlaceId,
            DistanceInKm = c.DistanceInKm,
        }).OrderBy(c => c.DistanceInKm).ToList();

        return Task.FromResult(list);
    }
}
