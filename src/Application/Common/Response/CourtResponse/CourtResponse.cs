using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Response.CourtResponse;
public class CourtResponse : IMapFrom<Court>
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public string? Description { get; set; }
    public string? CourtName { get; set; }
    public string? Address { get; set; }
    public string? GoogleMapURLs { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public string? PlaceId { get; set; }
    public double DistanceInKm { get; set; }
    public double FbStar { get; set; }
    public int CourtSubCount { get; set; }
    public int RentalNumber { get; set; }
    public decimal Price { get;set; }
    //public virtual IList<CourtSubdivision>? CourtSubdivision { get; set; }
}
public class CourtResponseV2 : IMapFrom<Court>
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public string? Description { get; set; }
    public string? CourtName { get; set; }
    public string? Address { get; set; }
    public string? GoogleMapURLs { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public DateTime Created { get; set; }
    public bool IsDelete { get; set; }
    public double? LatitudeDelta { get; set; }
    public double? LongitudeDelta { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    //public virtual IList<CourtSubdivision>? CourtSubdivision { get; set; }
}

public class CourtResponseV3 : IMapFrom<Court>
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public string? Description { get; set; }
    public string? CourtName { get; set; }
    public string? Address { get; set; }
    public string? GoogleMapURLs { get; set; }
    public string? WallpaperUrls { get; set; }
    public string? CoverImgUrls { get; set; }
    public List<string>? CourtImgsList { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public string? PlaceId { get; set; }
    public double DistanceInKm { get; set; }
    public double FbStar { get; set; }
    public double? LatitudeDelta { get; set; }
    public double? LongitudeDelta { get; set; }  
    public int CourtSubCount { get; set; }
    public int RentalNumber { get; set; }
    public decimal Price { get; set; }
    public List<CourtSubdivisionResponse>? CourtSubdivision { get; set; }
}

public class CourtResponseV4 : IMapFrom<Court>
{
    public Guid CourtId { get; set; }
    public string? CourtName { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public List<CourtSubdivisionV2>? CourtSubdivision { get; set; }
}

//For screen detail court
public class CourtResponseV5
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public string? Description { get; set; }
    public string? CourtName { get; set; }
    public string? Address { get; set; }
    public string? PlaceId { get; set; }
    public decimal? Price { get; set; }
    public decimal? FeedbackStarAvg { get; set; }
    public string? WallpaperUrls { get; set; }
    public string? CoverImgUrls { get; set; }
    public List<string>? CourtImgsList { get; set; }
    public int? FeedbackCount { get; set; }
    public int? RentingCount { get; set; }
    //public string? ImagesList { get; set; }
    public List<FeedbackResponseV2>? Feedbacks { get; set; } 
}