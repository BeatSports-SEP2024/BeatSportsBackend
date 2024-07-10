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
    public string? CourtName { get; set; }
    public string? Address { get; set; }
    public string? GoogleMapURLs { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public DateTime Created { get; set; }
    public bool IsDelete { get; set; }
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
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public string? PlaceId { get; set; }
    public double DistanceInKm { get; set; }
    public double FbStar { get; set; }
    public int CourtSubCount { get; set; }
    public int RentalNumber { get; set; }
    public decimal Price { get; set; }
    public List<CourtSubdivisionResponse>? CourtSubdivision { get; set; }
}
