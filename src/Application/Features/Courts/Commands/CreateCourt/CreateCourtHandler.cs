using System.Text.RegularExpressions;
using System;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
public class CreateCourtHandler : IRequestHandler<CreateCourtCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CreateCourtHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateCourtCommand request, CancellationToken cancellationToken)
    {
        // Check Owner
        var owner = _dbContext.Owners.Where(x => x.Id == request.OwnerId).SingleOrDefault();
        if(owner == null || owner.IsDelete)
        {
            throw new BadRequestException($"Owner with Owner ID:{request.OwnerId} does not exist or have been delele");
        }

        string pattern = @"@(?<lat1>-?\d+\.\d+),(?<long1>-?\d+\.\d+)|!3d(?<lat2>-?\d+\.\d+)!4d(?<long2>-?\d+\.\d+)";

        // Match the pattern
        var match = Regex.Match(request.GoogleMapURLs, pattern);
        var latitude = 0.0;
        var longitude = 0.0;

        if (match.Success)
        {
            // Extract the coordinates
            var lat1 = match.Groups["lat1"].Value;
            var long1 = match.Groups["long1"].Value;
            var lat2 = match.Groups["lat2"].Value;
            var long2 = match.Groups["long2"].Value;

            // Determine which coordinates to use
            latitude = double.Parse(string.IsNullOrEmpty(lat2) ? lat1 : lat2);
            longitude = double.Parse(string.IsNullOrEmpty(long2) ? long1 : long2);

        }

        var court = new Court()
        {
            Address = request.Address,
            CourtName = request.CourtName,
            Description = request.Description,
            GoogleMapURLs = request.GoogleMapURLs,
            OwnerId = request.OwnerId,
            TimeStart = request.TimeStart,
            TimeEnd = request.TimeEnd,
            Latitude = latitude,
            Longitude = longitude,
            ImageUrls = String.Join(",", request.ImageUrls),
            PlaceId = $"{latitude}, {longitude}",
            CourtSubdivision = new List<CourtSubdivision>()
        };

        if (request.CourtSubdivision != null && request.CourtSubdivision.Any())
        {
            foreach (var subdivision in request.CourtSubdivision)
            {
                court.CourtSubdivision.Add(new CourtSubdivision
                {
                    CourtId = court.Id,
                    //Description = subdivision.Description,
                    //ImageURL = subdivision.ImageURL,
                    IsActive = true,
                    BasePrice = subdivision.BasePrice,
                    CourtSubdivisionName = subdivision.CourtSubdivisionName,
                });
            }
        }

        _dbContext.Courts.Add(court);
        _dbContext.SaveChanges();

        return new BeatSportsResponse
        {
            Message = "Create successfully!"
        };
    }
}