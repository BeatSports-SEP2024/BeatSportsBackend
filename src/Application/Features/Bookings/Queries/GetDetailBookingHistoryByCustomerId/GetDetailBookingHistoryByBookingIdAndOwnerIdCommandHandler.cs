﻿using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailReadyForFinishBooking;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetDetailBookingHistoryByCustomerId;
public class GetDetailBookingHistoryByBookingIdAndOwnerIdCommand : IRequest<BookingHistoryDetailByCustomerId>
{
    public Guid BookingId { get; set; }
    public Guid OwnerId { get; set; }
}
public class GetDetailBookingHistoryByBookingIdAndOwnerIdCommandHandler : IRequestHandler<GetDetailBookingHistoryByBookingIdAndOwnerIdCommand, BookingHistoryDetailByCustomerId>
{
    private readonly IBeatSportsDbContext _dbContext;

    public GetDetailBookingHistoryByBookingIdAndOwnerIdCommandHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookingHistoryDetailByCustomerId> Handle(GetDetailBookingHistoryByBookingIdAndOwnerIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bookingExist = await (
            from booking in _dbContext.Bookings
            join customer in _dbContext.Customers on booking.CustomerId equals customer.Id
            join account in _dbContext.Accounts on customer.AccountId equals account.Id
            join subCourt in _dbContext.CourtSubdivisions on booking.CourtSubdivisionId equals subCourt.Id
            join court in _dbContext.Courts on subCourt.CourtId equals court.Id
            join campaign in _dbContext.Campaigns on booking.CampaignId equals campaign.Id into campaignJoin
            from campaign in campaignJoin.DefaultIfEmpty()

            where !booking.IsDelete && booking.Id == request.BookingId
            && court.OwnerId == request.OwnerId
            select new BookingHistoryDetailByCustomerId
            {
                BookingId = booking.Id,
                CustomerId = customer.Id,
                FirstName = account.FirstName,
                QRUrl = booking.QRUrlForCheckIn,

                LastName = account.LastName,
                PhoneNumber = account.PhoneNumber,
                CustomerAddress = customer.Address,
                UnixTimestampMinCancellation = booking.UnixTimestampMinCancellation,

                CampaignId = booking.CampaignId,
                CampaignName = campaign != null ? campaign.CampaignName : null,
                PercentDiscount = campaign != null ? campaign.PercentDiscount : (decimal?)null,
                MinValueApply = campaign != null ? campaign.MinValueApply : (decimal?)null,
                MaxValueDiscount = campaign != null ? campaign.MaxValueDiscount : (decimal?)null,


                CourtSubdivisionId = subCourt.Id,
                CourtSubName = subCourt.CourtSubdivisionName,
                BasePrice = subCourt.BasePrice,
                CourtId = court.Id,
                CourtName = court.CourtName,
                CourtAddress = court.Address,
                CourtWallImage = court.WallpaperUrls,
                CourtAvatarImage = court.CourtAvatarImgUrls,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                TotalPriceInTimePeriod = booking.TotalPriceInTimePeriod,
                TotalPriceDiscountCampaign = booking.TotalPriceDiscountCampaign,
                ListCourtByTimePeriod = !string.IsNullOrEmpty(booking.PayloadDescriptionPriceOfTimePeriod) ?
                    JsonConvert.DeserializeObject<List<CourtDetailInBookingDetailReadyForFinishBookingReponse>>(booking.PayloadDescriptionPriceOfTimePeriod) :
                    new List<CourtDetailInBookingDetailReadyForFinishBookingReponse>(),

                IsRoomBooking = booking.IsRoomBooking,
                IsDeposit = booking.IsDeposit,
                PlayingDate = booking.PlayingDate,
                StartTimePlaying = booking.StartTimePlaying,
                EndTimePlaying = booking.EndTimePlaying,
                BookingStatus = booking.BookingStatus,

            }).FirstOrDefaultAsync();
            if (bookingExist == null)
            {
                return null;
            }
            return bookingExist;
        }
        catch (Exception ex)
        {
            throw new NotFoundException("Not Found exception: " + ex.Message);

        }
    }
}
