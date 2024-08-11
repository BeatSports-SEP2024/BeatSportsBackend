using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimeCheckings.Commands;
public class CreateLockCourtSubdivisionCommand : IRequest<BeatSportsResponse>
{
    public Guid CourtSubdivisionId { get; set; }
    public TimeSpan StartTimeWantToLock { get; set; }
    public TimeSpan EndTimeWantToLock { get; set; }
    public DateTime DayWantToLock { get; set; }
}
public class CreateLockCourtSubdivisionCommandHandler : IRequestHandler<CreateLockCourtSubdivisionCommand, BeatSportsResponse>
{
    private IBeatSportsDbContext _dbContext;

    public CreateLockCourtSubdivisionCommandHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BeatSportsResponse> Handle(CreateLockCourtSubdivisionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var newTimeChecking = new TimeChecking()
            {
                CourtSubdivisionId = request.CourtSubdivisionId,
                DateBooking = request.DayWantToLock,
                StartTime = request.DayWantToLock.Date.Add(request.StartTimeWantToLock),
                EndTime = request.DayWantToLock.Date.Add(request.EndTimeWantToLock),
                IsLock = true,

            };
            _dbContext.TimeChecking.Add(newTimeChecking);
            await _dbContext.SaveChangesAsync();
            return new BeatSportsResponse
            {
                Message = "Lock sân thành công!"
            };
        }
        catch (Exception ex)
        {
            throw new BadRequestException("Lock sân không thành công!");
        }

    }
}
