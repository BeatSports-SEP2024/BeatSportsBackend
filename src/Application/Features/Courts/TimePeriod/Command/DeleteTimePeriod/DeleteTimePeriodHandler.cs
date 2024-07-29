using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using BeatSportsAPI.Application.Features.Hubs;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.DeleteTimePeriod;
public class DeleteTimePeriodHandler : IRequestHandler<DeleteTimePeriodCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IHubContext<TimePeriodHub> _hubContext;

    public DeleteTimePeriodHandler(IBeatSportsDbContext beatSportsDbContext, IHubContext<TimePeriodHub> hubContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _hubContext = hubContext;
    }

    public async Task<BeatSportsResponse> Handle(DeleteTimePeriodCommand request, CancellationToken cancellationToken)
    {
        var response = _beatSportsDbContext.TimePeriods
            .Where(tp => tp.Id == request.TimePeriodId && !tp.IsDelete)
            .FirstOrDefault();
        if (response == null)
        {
            throw new BadRequestException("Time Period does not existed");
        }
        response.IsDelete = true;
        _beatSportsDbContext.TimePeriods.Update(response);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        await _hubContext.Clients.All.SendAsync("DeleteTimePeriods");

        return new BeatSportsResponse
        {
            Message = "Delete Successfully"
        };
    }
}
