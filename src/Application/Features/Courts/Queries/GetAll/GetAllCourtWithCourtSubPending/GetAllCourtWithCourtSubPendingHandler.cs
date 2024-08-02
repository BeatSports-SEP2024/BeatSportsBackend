using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll.GetAllCourtWithCourtSubPending;
public class GetAllCourtWithCourtSubPendingHandler : IRequestHandler<GetAllCourtWithCourtSubPendingCommand, CourtResponseV8>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetAllCourtWithCourtSubPendingHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<CourtResponseV8> Handle(GetAllCourtWithCourtSubPendingCommand request, CancellationToken cancellationToken)
    {
        var pendingStatus = CourtSubdivisionCreatedStatus.Pending.ToString();

        var courtDetails = _beatSportsDbContext.Courts
            .Where(c => c.Id == request.CourtId && !c.IsDelete)
            .Include(cs => cs.CourtSubdivision)
                .ThenInclude(cs => cs.CourtSubdivisionSettings)
            .Include(f => f.Feedback)
            .ThenInclude(f => f.Booking)
                .ThenInclude(b => b.Customer)
                    .Include(c => c.Owner)
                        .ThenInclude(c => c.Account)
            .Select(c => new CourtResponseV8
            {
                Id = c.Id,
                CourtName = c.CourtName,
                OwnerName = c.Owner.Account.FirstName + " " + c.Owner.Account.LastName,
                Description = c.Description,
                OwnerAddress = c.Owner.Address,
                OwnerBio = c.Owner.Account.Bio,
                OwnerEmail = c.Owner.Account.Email,
                OwnerPhone = c.Owner.Account.PhoneNumber,
                OwnerGender = c.Owner.Account.Gender,
                OwnerDateOfBirth = c.Owner.Account.DateOfBirth,
                Address = c.Address,
                PlaceId = c.PlaceId,
                WallpaperUrls = c.WallpaperUrls,
                GoogleMapURLs = c.GoogleMapURLs,
                TimeStart = c.TimeStart,
                TimeEnd = c.TimeEnd,
                CourtSubCount = c.CourtSubdivision.Where(css => css.CreatedStatus.Equals(pendingStatus)).Count(),
                CoverImgUrls = c.CourtAvatarImgUrls, // Chuỗi gốc cho ảnh bìa
                CourtImgsList = ImageUrlSplitter.SplitImageUrls(c.ImageUrls),
                RentingCount = c.Feedback.Select(f => f.Booking).Distinct().Count(),
                FeedbackCount = c.Feedback.Count(),
                FeedbackStarAvg = c.Feedback.Any() ? c.Feedback.Average(x => x.FeedbackStar) : (decimal?)null,
                Price = c.CourtSubdivision.FirstOrDefault() != null ? c.CourtSubdivision.FirstOrDefault().BasePrice : (decimal?)null,
                Created = c.Created,

                CourtSubdivision = c.CourtSubdivision.Where(css => css.CreatedStatus == 0)
                    .Select(subCourt => new CourtSubdivisionV6
                    {
                        CourtSubdivisionId = subCourt.Id,
                        CourtSubdivisionName = subCourt.CourtSubdivisionName,
                        CourtSubType = subCourt.CourtSubdivisionDescription,
                        CreatedStatus = subCourt.CreatedStatus.ToString(), 
                        BasePrice = subCourt.BasePrice,
                        StartTime = c.TimeStart,
                        EndTime = c.TimeEnd,
                        CourtSubSettingResponses = new CourtSubSettingResponse
                        {
                            CourtSubSettingId = subCourt.CourtSubdivisionSettings.Id,
                            TypeSize = subCourt.CourtSubdivisionSettings.CourtType,
                            SportCategoryId = subCourt.CourtSubdivisionSettings.SportCategories.Id,
                            SportCategoryName = subCourt.CourtSubdivisionSettings.SportCategories.Name
                        }
                    }).ToList(),
                #region Feedback Response
                //Feedbacks = c.Feedback
                //    .Where(c => !c.IsDelete)
                //    .Select(c => new FeedbackResponseV2
                //    {
                //        FeedbackId = c.Id,
                //        FbStar = c.FeedbackStar,
                //        FeedbackContent = c.FeedbackContent,
                //        ProfilePictureUrl = c.Booking.Customer.Account.ProfilePictureURL,
                //        FullName = c.Booking.Customer.Account.FirstName + " " + c.Booking.Customer.Account.LastName,
                //        FeedbackSentTime = ParseTimeExtension.GetFormattedTime(DateTime.Now - c.Created),
                //    }).ToList(),
                #endregion
            })
            .FirstOrDefault();
        if(courtDetails == null)
        {
            throw new BadRequestException("Not have any record fit with CourtId");
        }
        return Task.FromResult(courtDetails);
    }
}
