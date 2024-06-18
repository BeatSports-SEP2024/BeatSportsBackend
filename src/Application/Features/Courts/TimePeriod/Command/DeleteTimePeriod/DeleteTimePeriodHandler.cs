using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.DeleteTimePeriod;
public class DeleteTimePeriodHandler : IRequestHandler<DeleteTimePeriodCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public DeleteTimePeriodHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
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
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return new BeatSportsResponse
        {
            Message = "Delete Successfully"
        };
    }
}
