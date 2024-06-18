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

        var query = new List<Court>();
        if (request.CourtId != Guid.Empty)
        {
             query = _dbContext.Courts
            .Where(x => !x.IsDelete && x.Id == request.CourtId)
            .Include(x => x.Feedback)
            .Include(x => x.CourtSubdivision)
            .ThenInclude(x => x.Bookings)
            .ToList();
        }
        else
        {
            query = _dbContext.Courts
            .Where(x => !x.IsDelete)
            .Include(x => x.Feedback)
            .Include(x => x.CourtSubdivision)
            .ThenInclude(x => x.Bookings)
            .ToList();
        }

        var list = new List<CourtResponse>();

        foreach (var c in query)
        {
            double starAvg = 0;
            decimal price = 0;
            int rentalNumber = 0;
            if(c.Feedback.Count != 0)
            {
                starAvg = (double)c.Feedback.Average(x => x.FeedbackStar);
                
            }

            if(c.CourtSubdivision.Count != 0)
            {
                price = c.CourtSubdivision.MinBy(c => c.BasePrice).BasePrice;
                rentalNumber = c.CourtSubdivision.Sum(x => x.Bookings.Where(c => c.BookingStatus.Equals("Approved")).Count());

            }

            
            list.Add(new CourtResponse
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
                FbStar = starAvg,
                CourtSubCount = c.CourtSubdivision.Count,
                Price = price,
                RentalNumber = rentalNumber
            });
        }

        if (request.Latitude != 0 && request.Longitude != 0)
        {
            //Goi service tinh khoang cach
            var result = distanceCal.GetDistancesAsync(request.Latitude, request.Longitude, query);

            int index = 0;
            result.Result.ForEach(c =>
            {
                list.ElementAt(index).DistanceInKm = c.DistanceInKm;
                index++;
            });

            list = list.OrderBy(c => c.DistanceInKm).ToList();
            return Task.FromResult(list);
        }

        return Task.FromResult(list);

    }
}
