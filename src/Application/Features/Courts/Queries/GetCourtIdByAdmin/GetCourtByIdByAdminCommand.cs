using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetCourtIdByAdmin;
public class GetCourtByIdByAdminCommand : IRequest<CourtResponseV7>
{
    public Guid CourtId { get; set; }
    public string? UsernameFilter { get; set; }
    public DateTime? FromTime { get; set; }
    public DateTime? ToTime { get; set; }
}

public class GetCourtByIdByAdminCommandHandler : IRequestHandler<GetCourtByIdByAdminCommand, CourtResponseV7>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetCourtByIdByAdminCommandHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public Task<CourtResponseV7> Handle(GetCourtByIdByAdminCommand request, CancellationToken cancellationToken)
    {
        //svar query = new List<Court>();

        var courtDetails = _beatSportsDbContext.Courts
            .Where(c => c.Id == request.CourtId)
                .Include(cs => cs.CourtSubdivision)
                    .ThenInclude(cs => cs.CourtSubdivisionSettings)
                        .ThenInclude(css => css.SportCategories)
                .Include(f => f.Feedback)
                    .ThenInclude(f => f.Booking)
                        .ThenInclude(b => b.Customer)
                .Include(c => c.Owner)
                        .ThenInclude(c => c.Account)
            .Select(c => new CourtResponseV7
            {
                Id = c.Id,
                CourtName = c.CourtName,
                OwnerName = c.Owner.Account.FirstName + " " + c.Owner.Account.LastName,
                Description = c.Description,
                Address = c.Address,
                PlaceId = c.PlaceId,
                WallpaperUrls = c.WallpaperUrls,
                GoogleMapURLs = c.GoogleMapURLs,
                TimeStart = c.TimeStart,
                TimeEnd = c.TimeEnd,
                CourtSubCount = c.CourtSubdivision.Count(),
                CoverImgUrls = c.CourtAvatarImgUrls, // Chuỗi gốc cho ảnh bìa
                CourtImgsList = ImageUrlSplitter.SplitImageUrls(c.ImageUrls),
                RentingCount = c.Feedback.Select(f => f.Booking).Distinct().Count(),
                FeedbackCount = c.Feedback.Count(),
                FeedbackStarAvg = c.Feedback.Any() ? c.Feedback.Average(x => x.FeedbackStar) : (decimal?)null,
                Price = c.CourtSubdivision.FirstOrDefault() != null ? c.CourtSubdivision.FirstOrDefault().BasePrice : (decimal?)null,

                SportCategoryList = c.CourtSubdivision
                        .Select(cs => cs.CourtSubdivisionSettings.SportCategories)
                        .Distinct()
                        .Select(sc => new SportCategoryResponse
                        {
                            SportId = sc.Id,
                            SportName = sc.Name
                        }).ToList(),

                CourtSubdivision = c.CourtSubdivision
                    .Select(subCourt => new CourtSubdivisionV7
                    {
                        CourtSubdivisionId = subCourt.Id,
                        CourtSubdivisionName = subCourt.CourtSubdivisionName,
                        CourtSubType = subCourt.CourtSubdivisionDescription,
                        BasePrice = subCourt.BasePrice,
                        StartTime = c.TimeStart,
                        EndTime = c.TimeEnd,
                        StatusCourtSubdivisor = subCourt.CreatedStatus.ToString(),
                        ReasonOfRejected = subCourt.ReasonOfRejected,
                        CourtSubSettingResponses = new CourtSubSettingResponse
                        {
                            CourtSubSettingId = subCourt.CourtSubdivisionSettings.Id,
                            TypeSize = subCourt.CourtSubdivisionSettings.CourtType,
                            SportCategoryId = subCourt.CourtSubdivisionSettings.SportCategories.Id,
                            SportCategoryName = subCourt.CourtSubdivisionSettings.SportCategories.Name
                        }
                    }).ToList(),

                Feedbacks = c.Feedback
                    .Where(c => !c.IsDelete)
                    .Select(c => new FeedbackResponseV2
                    {
                        FeedbackId = c.Id,
                        FbStar = c.FeedbackStar,
                        FeedbackContent = c.FeedbackContent,
                        ProfilePictureUrl = c.Booking.Customer.Account.ProfilePictureURL,
                        FullName = c.Booking.Customer.Account.FirstName + " " + c.Booking.Customer.Account.LastName,
                        FeedbackSentTime = ParseTimeExtension.GetFormattedTime(DateTime.Now - c.Created),
                        FeedbackDate = c.Created,
                    }).ToList(),
            })
            .FirstOrDefault();
        return Task.FromResult(courtDetails);
    }
}