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
        var existedCourt = _beatSportsDbContext.CourtSubdivisions
            .Where(c => c.Id == request.CourtSubdivisionId && !c.IsDelete)
            .FirstOrDefault();
        if (existedCourt == null)
        {
            throw new NotFoundException($"{request.CourtSubdivisionId} is not existed");
        }
        foreach (var (start, end) in TimeUtils.GenerateTimeSlots(request.StartTime, request.EndTime))
        {
            var newTimePeriod = new Domain.Entities.CourtEntity.TimePeriod
            {
                CourtSubdivisionId = request.CourtSubdivisionId,
                //Description = request.Description,
                StartTime = start,
                EndTime = end,
                //RateMultiplier = request.RateMultiplier
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
