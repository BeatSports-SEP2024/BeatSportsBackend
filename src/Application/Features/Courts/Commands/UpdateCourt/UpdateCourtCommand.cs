using System.ComponentModel.DataAnnotations;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Commands.UpdateCourt;
public class UpdateCourtCommand : IRequest<BeatSportsResponse>
{
    public Guid OwnerId { get; set; }
    public Guid CourtId { get; set; }
    public string Description { get; set; } = null!;
    public string CourtName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string GoogleMapURLs { get; set; } = null!;
    public TimeSpan TimeStart { get; set; }
    public TimeSpan TimeEnd { get; set; }
    [Required]
    public string PlaceId { get; set; } = null!;
}
