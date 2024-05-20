using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.CreateCourtSubdivision;
public class CreateCourtSubdivisionCommand : IRequest<BeatSportsResponse>
{
    public Guid CourtId { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public decimal BasePrice { get; set; }
}
