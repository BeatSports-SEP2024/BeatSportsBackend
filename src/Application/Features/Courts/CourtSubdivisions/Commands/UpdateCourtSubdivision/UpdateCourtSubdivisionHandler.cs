using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.UpdateCourtSubdivision;
public class UpdateCourtSubdivisionHandler : IRequestHandler<UpdateCourtSubdivisionCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public UpdateCourtSubdivisionHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponse> Handle(UpdateCourtSubdivisionCommand request, CancellationToken cancellationToken)
    {
        //Check exist
        var cs = _dbContext.CourtSubdivisions.Where(x => x.Id == request.CourtSubdivisionId && !x.IsDelete).SingleOrDefault();
        if(cs == null)
        {
            throw new NotFoundException($"Do not find court subdivision have ID: {request.CourtSubdivisionId}");
        }
        //cs.Description = request.Description;
        //cs.ImageURL = request.ImageURL;
        cs.BasePrice = request.BasePrice;
        cs.CourtSubdivisionName = request.CourtSubdivisionName;

        _dbContext.CourtSubdivisions.Update(cs);
        _dbContext.SaveChanges();
        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Update successfully"
        });
    }
}
