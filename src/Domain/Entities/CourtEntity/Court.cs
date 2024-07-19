using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.Room;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class Court : BaseAuditableEntity
{
    [ForeignKey("Owner")]
    public Guid OwnerId { get; set; }
    /*public int Capacity { get; set; }*/
    public string? Description { get; set; }
    public string? CourtName { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    //Anh bia
    public string? WallpaperUrls { get; set; }
    //Anh Dai dien cua san
    public string? CourtAvatarImgUrls { get; set; }
    //Zoom in, out google map
    public double? LatitudeDelta { get; set; }
    public double? LongitudeDelta { get; set; }
    public string? GoogleMapURLs { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public string? ImageUrls { get; set; }
    /// <summary>
    /// Unique key
    /// </summary>
    [Required]
    public string? PlaceId { get; set; }

    public virtual Owner Owner { get; set; } = null!;
    //public virtual IList<RoomMatch> RoomMatch { get; set; } = null!;
    public virtual IList<Campaign>? Campaigns { get; set; }
    //public virtual IList<TimePeriod>? TimePeriods { get; set; }
    public virtual IList<Feedback>? Feedback { get; set; }
    //public virtual IList<CourtSportCategory>? CourtCategories { get; set; }
    public virtual IList<CourtSubdivision>? CourtSubdivision { get; set; }
    public virtual IList<TimePeriod>? TimePeriods { get; set; }
}
