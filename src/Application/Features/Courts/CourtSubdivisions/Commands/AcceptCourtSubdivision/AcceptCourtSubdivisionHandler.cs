using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.AcceptCourtSubdivision;
public class AcceptCourtSubdivisionHandler : IRequestHandler<AcceptCourtSubdivisionCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public AcceptCourtSubdivisionHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<BeatSportsResponse> Handle(AcceptCourtSubdivisionCommand request, CancellationToken cancellationToken)
    {
        var courtSub = _beatSportsDbContext.CourtSubdivisions
            .Where(cs => !cs.IsDelete && cs.Id == request.CourtSubdivisionId)
            .FirstOrDefault();

        if(courtSub == null) 
        {
            throw new BadRequestException("Court Sub is not existed");
        }

        courtSub.CreatedStatus = request.Status.ToString();
        courtSub.ReasonOfRejected = request.ReasonOfReject;

        _beatSportsDbContext.CourtSubdivisions.Update(courtSub);
        _beatSportsDbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Court Subdivision is approved/rejected",
        });
    }
}