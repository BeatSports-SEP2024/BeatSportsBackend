using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.UpdateCourtSubdivision;
public class UpdateCourtSubdivisionCommand : IRequest<BeatSportsResponse>
{
    public Guid CourtSubdivisionId { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public decimal BasePrice { get; set; }
}
