using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailByCustomer;
public class GetBookingDetailByCustomerHandler : IRequestHandler<GetBookingDetailByCustomerCommand, List<BookingDetailByCustomer>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetBookingDetailByCustomerHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<List<BookingDetailByCustomer>> Handle(GetBookingDetailByCustomerCommand request, CancellationToken cancellationToken)
    {
        var query = await (
            from booking in _beatSportsDbContext.Bookings
            where !booking.IsDelete && booking.CustomerId == request.CustomerId
            join customer in _beatSportsDbContext.Customers on booking.CustomerId equals customer.Id
            join account in _beatSportsDbContext.Accounts on booking.Customer.Account.Id equals account.Id
            join courtSub in _beatSportsDbContext.CourtSubdivisions on booking.CourtSubdivisionId equals courtSub.Id
            join court in _beatSportsDbContext.Courts on courtSub.CourtId equals court.Id
            join campaign in _beatSportsDbContext.Campaigns on booking.Campaign.Id equals campaign.Id into campaignJoin
            from campaign in campaignJoin.DefaultIfEmpty()
            select new BookingDetailByCustomer
            {
                BookingId = booking.Id,
                CustomerId = customer.Id,
                CampaignId = campaign != null ? campaign.Id : (Guid?)null,
                CampaignName = campaign.CampaignName,
                //MaxValueDiscount = campaign.MaxValueDiscount,
                //MinValueApply = campaign.MinValueApply,
                FullName = customer.Account.FirstName + " " + customer.Account.LastName,
                CustomerAddress = customer.Address,
                CustomerPhone = customer.Account.PhoneNumber,
                CourtId = court.Id,
                CourtName = court.CourtName,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                PlayingDate = booking.PlayingDate,
                StartTimePlaying = booking.StartTimePlaying,
                EndTimePlaying = booking.EndTimePlaying,
                BookingStatus = booking.BookingStatus,
                DiscountPrice = campaign != null && courtSub.BasePrice >= campaign.MinValueApply
                ? Math.Min(courtSub.BasePrice * campaign.PercentDiscount / 100, campaign.MaxValueDiscount)
                : 0
            }).ToListAsync(cancellationToken);
        return query;
    }
}