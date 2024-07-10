using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAllCourtsByOwnerId;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.MapBox;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetListCourtsNearBy;
public class GetListCourtsNearByHandler : IRequestHandler<GetListCourtsNearByCommand, List<CourtResponseV3>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetListCourtsNearByHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<List<CourtResponseV3>> Handle(GetListCourtsNearByCommand request, CancellationToken cancellationToken)
    {
        var distanceCal = new DistanceCalculation();

        if(request.KeyWords == null)
        {
            request.KeyWords = "";
        }

        var query = new List<Court>();

        if (request.CourtId != Guid.Empty)
        {
             query = _dbContext.Courts
            .Where(x => !x.IsDelete && x.Id == request.CourtId && x.CourtName.Contains(request.KeyWords) && x.Address.Contains(request.KeyWords))
            .Include(x => x.Owner)
            .Include(x => x.Feedback)
            .Include(x => x.CourtSubdivision)
            .ThenInclude(x => x.Bookings)
            .ToList();
        }
        else
        {
            query = _dbContext.Courts
            .Where(x => !x.IsDelete && x.CourtName.Contains(request.KeyWords) && x.Address.Contains(request.KeyWords))
            .Include(x => x.Owner)
            .Include(x => x.Feedback)
            .Include(x => x.CourtSubdivision)
            .ThenInclude(x => x.Bookings)
            .ToList();
        }

        var list = new List<CourtResponseV3>();

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

            var listCourtSub = c.CourtSubdivision.Select(b => new CourtSubdivisionResponse
            {
                Id = b.Id,
                CourtId = b.CourtId,
                CourtSubdivisionName = b.CourtSubdivisionName,
                BasePrice = b.BasePrice,
                IsActive = b.IsActive,
            }).ToList();

            list.Add(new CourtResponseV3
            {
                Id = c.Id,
                OwnerName = "Hi lko",
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
                RentalNumber = rentalNumber,
                CourtSubdivision = listCourtSub,
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
