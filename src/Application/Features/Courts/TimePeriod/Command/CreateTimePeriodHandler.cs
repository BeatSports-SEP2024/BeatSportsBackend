﻿using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command;
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
            throw new NotFoundException("Cannot find courtId or court id does not exist");
        }
        var newTimePeriod = new BeatSportsAPI.Domain.Entities.CourtEntity.TimePeriod
        {
            CourtId = request.CourtId,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RateMultiplier = request.RateMultiplier,
        };
        await _beatSportsDbContext.TimePeriods.AddAsync(newTimePeriod, cancellationToken);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse 
        {
            Message = "Create Time Period Successfully"
        };
    }
}