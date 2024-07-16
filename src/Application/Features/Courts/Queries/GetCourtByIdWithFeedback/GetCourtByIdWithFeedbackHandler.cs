using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.MapBox;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetCourtByIdWithFeedback;
public class GetCourtByIdWithFeedbackHandler : IRequestHandler<GetCourtByIdWithFeedbackCommand, CourtResponseV5>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetCourtByIdWithFeedbackHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<CourtResponseV5> Handle(GetCourtByIdWithFeedbackCommand request, CancellationToken cancellationToken)
    {
        var courtDetails = _beatSportsDbContext.Courts
            .Where(c => c.Id == request.CourtId)
            .Include(cs => cs.CourtSubdivision)
            .Include(f => f.Feedback)
            .ThenInclude(f => f.Booking)
                .ThenInclude(b => b.Customer)
                    .Include(c => c.Owner)
                        .ThenInclude(c => c.Account)
            .Select(c => new CourtResponseV5
            {
                Id = c.Id,
                CourtName = c.CourtName,
                OwnerName = c.Owner.Account.FirstName + " " + c.Owner.Account.LastName,
                Description = c.Description,
                Address = c.Address,
                PlaceId = c.PlaceId,
                ImagesList = c.ImageUrls,
                RentingCount = c.Feedback.Select(f => f.Booking).Distinct().Count(),
                FeedbackCount = c.Feedback.Count(),
                FeedbackStarAvg = c.Feedback.Any() ? c.Feedback.Average(x => x.FeedbackStar) : (decimal?)null,
                Price = c.CourtSubdivision.FirstOrDefault() != null ? c.CourtSubdivision.FirstOrDefault().BasePrice : (decimal?)null,
                Feedbacks = c.Feedback
                    .Where(c => !c.IsDelete)
                    .Select(c => new FeedbackResponseV2
                    {
                        CourtId = c.Id,
                        FeedbackStar = c.FeedbackStar,
                        FeedbackContent = c.FeedbackContent,
                        ProfilePictureUrl = c.Booking.Customer.Account.ProfilePictureURL,
                        FullName = c.Booking.Customer.Account.FirstName + " " + c.Booking.Customer.Account.LastName
                    }).ToList(),
            })
            .FirstOrDefault();
        return Task.FromResult(courtDetails);
    }
}