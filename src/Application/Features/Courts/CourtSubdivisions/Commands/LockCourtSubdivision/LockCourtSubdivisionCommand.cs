using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.LockCourtSubdivision;
public class LockCourtSubdivisionCommand : IRequest<BeatSportsResponse>
{
    public Guid CourtSubdivisionId { get; set; }
    public bool IsActive { get; set; }
}
