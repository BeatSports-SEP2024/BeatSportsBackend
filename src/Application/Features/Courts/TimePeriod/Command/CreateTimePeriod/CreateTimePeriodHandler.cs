using System;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.CreateTimePeriod;
public class CreateTimePeriodHandler : IRequestHandler<CreateTimePeriodCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    public CreateTimePeriodHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateTimePeriodCommand request, CancellationToken cancellationToken)
    {
        var existedCourt = _beatSportsDbContext.Courts
            .Where(c => c.Id == request.CourtId && !c.IsDelete)
            .FirstOrDefault();

        if (existedCourt == null)
        {
            throw new NotFoundException($"{request.CourtId} is not existed");
        }

        var timePeriods = _beatSportsDbContext.TimePeriods
            .Where(tp => tp.CourtId == request.CourtId && !tp.IsDelete).ToList();

        foreach (var (start, end) in TimeUtils.GenerateTimeSlots(request.StartTime, request.EndTime))
        {
            if (timePeriods.Any(tp => tp.StartTime < end && start < tp.EndTime))
            {
                throw new BadRequestException("Time period overlaps with existing period. Please check and try again.");
            }

            var newTimePeriod = new Domain.Entities.CourtEntity.TimePeriod
            {
                CourtId = request.CourtId,
                StartTime = start,
                EndTime = end
            };

            await _beatSportsDbContext.TimePeriods.AddAsync(newTimePeriod, cancellationToken);
        }

        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = "Create Time Period Successfully"
        };
    }
}