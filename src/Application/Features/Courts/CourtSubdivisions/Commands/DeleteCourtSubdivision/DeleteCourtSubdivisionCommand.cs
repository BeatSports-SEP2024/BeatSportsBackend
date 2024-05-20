using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.DeleteCourtSubdivision;
public class DeleteCourtSubdivisionCommand : IRequest<BeatSportsResponse>
{
    public Guid CourtSubdivisionId {  get; set; }
}
