using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetById;
public class GetCourtByIdCommand : IRequest<CourtResponse>
{
    public Guid CourtId { get; set; }
}
