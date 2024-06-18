﻿using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.Room;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class CourtSubdivision : BaseAuditableEntity
{
    [ForeignKey("Court")]
    public Guid CourtId { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public decimal BasePrice { get; set; }
    public string? CourtSubdivisionName { get; set; }

    public virtual Court Court { get; set; }
    public virtual IList<TimeChecking> TimeCheckings { get; set; }
    public virtual IList<Booking> Bookings { get; set; } = null!;
    public virtual IList<CourtSportCategory>? CourtSportCategories { get; set; }
    public virtual IList<TimePeriod>? TimePeriods { get; set; }
}
