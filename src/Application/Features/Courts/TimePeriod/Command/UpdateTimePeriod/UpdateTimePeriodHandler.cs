/*using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using BeatSportsAPI.Application.Common.Exceptions;
using System.Reflection;
using BeatSportsAPI.Application.Common.Ultilities;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.UpdateTimePeriod;
public class UpdateTimePeriodHandler : IRequestHandler<UpdateTimePeriodCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public UpdateTimePeriodHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(UpdateTimePeriodCommand request, CancellationToken cancellationToken)
    {
        //check that time period is existed or not
        var existingTimePeriod = _beatSportsDbContext.TimePeriods
            .FirstOrDefault(tp => tp.Id == request.TimePeriodId && tp.CourtId == request.CourtId && !tp.IsDelete);

        if (existingTimePeriod == null)
        {
            throw new NotFoundException("Time period not found.");
        }
        //list time period, check is overlaps or not
        var timePeriods = _beatSportsDbContext.TimePeriods
            .Where(tp => tp.CourtId == request.CourtId && tp.Id != request.TimePeriodId && !tp.IsDelete).ToList();

        foreach (var (start, end) in TimeUtils.GenerateTimeSlots(request.StartTime, request.EndTime))
        {
            if (timePeriods.Any(tp => tp.StartTime < end && start < tp.EndTime))
            {
                throw new BadRequestException("Time period overlaps with existing period. Please check and try again.");
            }
        }

        var (newStart, newEnd) = TimeUtils.GenerateTimeSlots(request.StartTime, request.EndTime).FirstOrDefault();

        existingTimePeriod.StartTime = newStart;
        existingTimePeriod.EndTime = newEnd;
        existingTimePeriod.Description = request.Description;
        existingTimePeriod.RateMultiplier = request.RateMultiplier;

        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = "Time Period Updated Successfully"
        };
    }
}*/