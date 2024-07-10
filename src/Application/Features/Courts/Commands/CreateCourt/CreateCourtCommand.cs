using System.ComponentModel.DataAnnotations;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
public class CreateCourtCommand : IRequest<BeatSportsResponse>
{
    public Guid OwnerId { get; set; }
    public string Description { get; set; } = null!;
    public string CourtName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string GoogleMapURLs { get; set; } = null!;
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    public string ImageUrls { get; set; }
  
    
    public ICollection<CourtSubdivisionDto>? CourtSubdivision { get; set; }

    public class CourtSubdivisionDto
    {
        public bool IsActive { get; set; }
        public decimal BasePrice { get; set; }
        public string CourtSubdivisionName { get; set; } = string.Empty;
        public string CategorySportName { get; set; }
    }
}
