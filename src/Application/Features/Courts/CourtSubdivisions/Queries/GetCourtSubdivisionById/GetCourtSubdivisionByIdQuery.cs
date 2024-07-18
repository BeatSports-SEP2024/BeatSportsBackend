using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionById;
public class GetCourtSubdivisionByIdQuery : IRequest<CourtSubdivisionV5>
{
    public Guid CourtSubdivisionId { get; set; }
}
