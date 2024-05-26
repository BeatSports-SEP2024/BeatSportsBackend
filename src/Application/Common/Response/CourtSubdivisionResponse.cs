﻿using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Application.Common.Response;
public class CourtSubdivisionResponse : IMapFrom<CourtSubdivision>
{
    public Guid Id { get; set; }
    public Guid CourtId { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public decimal BasePrice { get; set; }
}