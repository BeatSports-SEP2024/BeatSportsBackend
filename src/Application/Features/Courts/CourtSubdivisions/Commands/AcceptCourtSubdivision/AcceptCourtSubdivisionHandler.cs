using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
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
        var courtSubs = _beatSportsDbContext.CourtSubdivisions
            .Where(cs => !cs.IsDelete && cs.CourtId == request.CourtId)
            .ToList();

        if (!courtSubs.Any())
        {
            throw new BadRequestException("Court Sub is not existed");
        }

        foreach (var courtSub in courtSubs)
        {
            if (request.Status == StatusEnums.Accepted)
            {
                courtSub.CreatedStatus = (CourtSubdivisionCreatedStatus)StatusEnums.Accepted;
                courtSub.ReasonOfRejected = "";
            }
            if (request.Status == StatusEnums.Rejected)
            {
                courtSub.CreatedStatus = (CourtSubdivisionCreatedStatus)StatusEnums.Rejected;
                courtSub.ReasonOfRejected = request.ReasonOfReject;
            }
        }

        _beatSportsDbContext.CourtSubdivisions.UpdateRange(courtSubs);
        _beatSportsDbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Court Subdivision is approved/rejected",
        });
    }
}