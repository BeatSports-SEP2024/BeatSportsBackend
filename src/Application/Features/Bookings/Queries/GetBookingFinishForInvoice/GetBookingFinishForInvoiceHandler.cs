using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingFinishForInvoice;
public class GetBookingFinishForInvoiceHandler : IRequestHandler<GetBookingFinishForInvoiceQuery, List<BookingFinishForInvoiceResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetBookingFinishForInvoiceHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<List<BookingFinishForInvoiceResponse>> Handle(GetBookingFinishForInvoiceQuery request, CancellationToken cancellationToken)
    {
        var dateList = Enumerable.Range(0, (request.DayEnd - request.DayStart).Days + 1)
                     .Select(offset => request.DayStart.AddDays(offset))
                     .ToList();

        var query = await (
            from booking in _beatSportsDbContext.Bookings
            where !booking.IsDelete
                && booking.CourtSubdivision.Court.Id == request.CourtId
                && booking.BookingDate >= request.DayStart
                && booking.BookingDate <= request.DayEnd
            join customer in _beatSportsDbContext.Customers on booking.CustomerId equals customer.Id
            join account in _beatSportsDbContext.Accounts on customer.Account.Id equals account.Id
            join courtSub in _beatSportsDbContext.CourtSubdivisions on booking.CourtSubdivisionId equals courtSub.Id

            select new
            {
                booking,
                customer,
                account,
                courtSub
            }).ToListAsync();

        var response = dateList.Select((date, index) => new BookingFinishForInvoiceResponse
        {
            IdFlag = index + 1,
            DateCheck = date.ToString("yyyy-MM-dd hh:mm:ss"),
            TotalPriceOfDay = query.Where(q => q.booking.BookingDate.Date == date).Sum(q => q.booking.TotalAmount),
            ListBooked = query.Where(q => q.booking.BookingDate.Date == date).Select(q => new BookingOfCourtInDay
            {
                BookingId = q.booking.Id,
                CourtSubdivisionId = q.booking.CourtSubdivisionId,
                CustomerId = q.booking.CustomerId,
                FullNameOfCustomer = q.account.FirstName + " " + q.account.LastName,
                CourtSubdivisionName = q.courtSub.CourtSubdivisionName,
                DayTimeBooking = q.booking.BookingDate.ToString("yyyy-MM-dd hh:mm:ss"),
                //TotalPrice = query.Where(q => q.booking.BookingDate.Date == date).Sum(q => q.booking.TotalAmount)
                TotalPrice = q.booking.TotalAmount,
            }).OrderByDescending(x => x.DayTimeBooking).ToList(),
        }).ToList();
        // Lọc ra những response có ListBooked.Any() = true
        var filteredResponse = response.Where(r => r.ListBooked.Any()).ToList();

        return filteredResponse;
    }
}