using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Response.CourtResponse;
public class CourtWithDetailResponse : IMapFrom<Court>
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string? Description { get; set; }
    public string? CourtName { get; set; }
    public string? Address { get; set; }
    public string? GoogleMapURLs { get; set; }
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public string? PlaceId { get; set; }
    public List<decimal> BasePrice { get; set; }
    [EnumDataType(typeof(SportCategoriesEnums))]
    public List<SportCategoriesEnums>? SportCategoriesEnums { get; set; }
}
