using System.Text.RegularExpressions;
using System;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BeatSportsAPI.Application.Common.Ultilities;

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
        var owner = _dbContext.Owners
            .Where(x => x.Id == request.OwnerId && !x.IsDelete).SingleOrDefault();
        if (owner == null)
        {
            throw new BadRequestException($"Owner with Owner ID: {request.OwnerId} does not exist");
        }

        string pattern = @"@(?<lat1>-?\d+\.\d+),(?<long1>-?\d+\.\d+)|!3d(?<lat2>-?\d+\.\d+)!4d(?<long2>-?\d+\.\d+)";
        var match = Regex.Match(request.GoogleMapURLs, pattern);
        var latitude = 0.0;
        var longitude = 0.0;

        if (match.Success)
        {
            latitude = double.Parse(string.IsNullOrEmpty(match.Groups["lat2"].Value) ? match.Groups["lat1"].Value : match.Groups["lat2"].Value);
            longitude = double.Parse(string.IsNullOrEmpty(match.Groups["long2"].Value) ? match.Groups["long1"].Value : match.Groups["long2"].Value);
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
            LatitudeDelta = 0.01,
            LongitudeDelta = 0.01,
            ImageUrls = String.Join(",", request.ImageUrls),
            WallpaperUrls = "https://res.cloudinary.com/dcbkmwm3v/image/upload/v1721128315/id6u4ckwjcayygtqgyze.png",
            PlaceId = $"{latitude}, {longitude}",
            CourtSubdivision = new List<CourtSubdivision>()
        };

        if (request.CourtSubdivision != null)
        {
            foreach (var subdivision in request.CourtSubdivision)
            {
                Guid settingId = subdivision.CourtSubdivisionSettingId;
                var settings = _dbContext.CourtSubdivisionSettings.FirstOrDefault(s => s.Id == settingId);

                if (settings == null)
                {
                    throw new BadRequestException("This court config does not exist");
                }

                string extractedText = StringExtraction.ExtractText(settings.CourtType);

                if(extractedText == null) 
                {
                    extractedText = ""; //Default is space when do not have string fit conditional
                }

                court.CourtSubdivision.Add(new CourtSubdivision
                {
                    CourtId = court.Id,
                    CourtSubdivisionDescription = extractedText,
                    IsActive = true,
                    BasePrice = subdivision.BasePrice,
                    CourtSubdivisionName = subdivision.CourtSubdivisionName,
                    CourtSubdivisionSettings = settings
                });
            }
        }

        _dbContext.Courts.Add(court);
        await _dbContext.SaveChangesAsync();

        return new BeatSportsResponse
        {
            Message = "Create successfully!"
        };
    }
}