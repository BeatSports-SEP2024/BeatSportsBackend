using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using BeatSportsAPI.Application.Common.Exceptions;
using System.Reflection;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Application.Features.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.UpdateTimePeriod;
public class UpdateTimePeriodHandler : IRequestHandler<UpdateTimePeriodCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IHubContext<TimePeriodHub> _hubContext;

    public UpdateTimePeriodHandler(IBeatSportsDbContext beatSportsDbContext, IHubContext<TimePeriodHub> hubContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _hubContext = hubContext;
    }

    public async Task<BeatSportsResponse> Handle(UpdateTimePeriodCommand request, CancellationToken cancellationToken)
    {
        //check that time period is existed or not
        var existingTimePeriod = _beatSportsDbContext.TimePeriods
            .FirstOrDefault(tp => tp.Id == request.TimePeriodId && !tp.IsDelete);

        if (existingTimePeriod == null)
        {
            throw new NotFoundException("Time period not found.");
        }
        existingTimePeriod.PriceAdjustment = request.PriceAdjustment;
        existingTimePeriod.Description = request.Description;
        _beatSportsDbContext.TimePeriods.Update(existingTimePeriod);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        await _hubContext.Clients.All.SendAsync("UpdateTimePeriods");
        return new BeatSportsResponse
        {
            Message = "Time Period Updated Successfully"
        };
    }
}