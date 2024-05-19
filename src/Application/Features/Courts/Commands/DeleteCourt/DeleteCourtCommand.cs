using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Commands.DeleteCourt;
public class DeleteCourtCommand : IRequest<BeatSportsResponse>
{
    public Guid CourtId { get; set; }
}
