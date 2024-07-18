using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Application.Common.Response;
public class CourtSubdivisionResponse : IMapFrom<CourtSubdivision>
{
    public Guid Id { get; set; }
    public Guid CourtId { get; set; }
    public string CourtName { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public decimal BasePrice { get; set; }
    public string? CourtSubdivisionName { get; set; }
}

public class CourtSubdivisionV2 : IMapFrom<CourtSubdivision>
{
    public Guid CourtSubdivisionId { get; set; }
    public string? CourtSubdivisionName { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class CourtSubdivisionResponseV3 : IMapFrom<CourtSubdivision>
{
    public Guid Id { get; set; }
    public Guid CourtId { get; set; }
    public string CourtName { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public decimal BasePrice { get; set; }
    public string? CourtSubdivisionName { get; set; }
    public string? Status { get; set; }
}

public class CourtSubdivisionV4 : IMapFrom<CourtSubdivision>
{
    public Guid CourtSubdivisionId { get; set; }
    public string? CourtSubdivisionName { get; set; }
    //public string? CourtSubType { get; set; }
    public decimal BasePrice { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    //public CourtSubSettingResponse? CourtSubSettingResponses { get; set; }
}
public class CourtSubdivisionV5
{
    public Guid CourtSubId { get; set; }
    public string? CourtSubName { get; set; }
    public string? CourtDescription { get; set; }
    public string? CourtSubDescription { get; set; }
    public decimal? BasePrice { get; set; }
    public string? Status { get; set; }
    public List<string>? ImgUrls { get; set; }
    public string? CourtType { get; set; }
    public string? SportCategories { get; set; }
    public string? Address { get; set; }
    public string? OwnerFullName { get; set; }
}