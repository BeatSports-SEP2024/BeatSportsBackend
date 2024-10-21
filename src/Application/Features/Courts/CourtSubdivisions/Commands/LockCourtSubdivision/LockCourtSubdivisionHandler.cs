using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.LockCourtSubdivision;
public class LockCourtSubdivisionHandler : IRequestHandler<LockCourtSubdivisionCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public LockCourtSubdivisionHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(LockCourtSubdivisionCommand request, CancellationToken cancellationToken)
    {
        //Check exist
        var cs = _dbContext.CourtSubdivisions.Where(x => x.Id == request.CourtSubdivisionId && !x.IsDelete).SingleOrDefault();
        if (cs == null)
        {
            throw new NotFoundException($"Do not find court subdivision have ID: {request.CourtSubdivisionId}");
        }
        cs.IsActive = request.IsActive;
        _dbContext.CourtSubdivisions.Update(cs);
        _dbContext.SaveChanges();
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Action successfully"
        });
    }
}
