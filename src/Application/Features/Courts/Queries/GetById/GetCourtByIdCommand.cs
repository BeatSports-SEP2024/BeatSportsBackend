using BeatSportsAPI.Application.Common.Response.CourtResponse;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetById;
public class GetCourtByIdCommand : IRequest<CourtResponseV3>
{
    public Guid CourtId { get; set; }
}
