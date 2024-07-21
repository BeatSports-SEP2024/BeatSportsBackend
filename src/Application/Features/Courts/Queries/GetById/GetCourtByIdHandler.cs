using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Services.MapBox;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetById;
public class GetCourtByIdHandler : IRequestHandler<GetCourtByIdCommand, CourtResponseV5>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetCourtByIdHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<CourtResponseV5> Handle(GetCourtByIdCommand request, CancellationToken cancellationToken)
    {
        var courtDetails = _beatSportsDbContext.Courts
            .Where(c => c.Id == request.CourtId)
            .Include(c => c.Campaigns)
            .Include(cs => cs.CourtSubdivision)
                .ThenInclude(cs => cs.CourtSubdivisionSettings)
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

                CourtSubSettingResponses = c.CourtSubdivision
                    .GroupBy(cs => cs.CourtSubdivisionSettings.Id)
                    .Select(g => new CourtSubSettingV2
                    {
                        CourtSubType = g.First().CourtSubdivisionDescription,
                        CourtSubSettingId = g.First().CourtSubdivisionSettings.Id,
                        TypeSize = g.First().CourtSubdivisionSettings.CourtType,
                        SportCategoryId = g.First().CourtSubdivisionSettings.SportCategories.Id,
                        SportCategoryName = g.First().CourtSubdivisionSettings.SportCategories.Name,
                        CourtSubdivision = g
                        .Select(subCourt => new CourtSubdivisionV4
                        {
                            CourtSubdivisionId = subCourt.Id,
                            CourtSubdivisionName = subCourt.CourtSubdivisionName,
                            //CourtSubType = subCourt.CourtSubdivisionDescription,
                            BasePrice = subCourt.BasePrice,
                            StartTime = c.TimeStart,
                            EndTime = c.TimeEnd,
                            CreatedStatus = subCourt.CreatedStatus.ToString()
                        }).ToList()
                    }).ToList(),

                CourtCampaignResponses = c.Campaigns
                    .Where(c => !c.IsDelete)
                    .Select(c => new CampaignResponseV6
                    {
                        Id = c.Id,
                        CourtId = c.CourtId,
                        ExpireCampaign = (c.EndDateApplying - DateTime.Now).ToString(),
                        MaxValueDiscount = c.MaxValueDiscount,
                        MinValueApply = c.MinValueApply,
                        PercentDiscount = c.PercentDiscount,
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
                    }).ToList(),
            })
            .FirstOrDefault();

        if (courtDetails == null)
        {
            throw new BadRequestException($"Court with ID {request.CourtId} not found.");
        }
        return Task.FromResult(courtDetails);
    }
}