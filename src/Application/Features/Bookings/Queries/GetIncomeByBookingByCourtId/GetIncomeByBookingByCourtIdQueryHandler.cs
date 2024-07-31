using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingFinishForInvoice;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetIncomeByBookingByCourtId;
public class GetIncomeByBookingByCourtIdQueryHandler : IRequestHandler<GetIncomeByBookingByCourtIdQuery, List<IncomeByBookingResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;

    public GetIncomeByBookingByCourtIdQueryHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<IncomeByBookingResponse>> Handle(GetIncomeByBookingByCourtIdQuery request, CancellationToken cancellationToken)
    {
        var dateList = Enumerable.Range(0, (request.DayEnd - request.DayStart).Days + 1)
                     .Select(offset => request.DayStart.AddDays(offset))
                     .ToList();

        var query = await
            (
                from booking in _dbContext.Bookings
                where !booking.IsDelete
                    && booking.CourtSubdivision.Court.Id == request.CourtId
                    && booking.BookingDate.Date >= request.DayStart.Date
                    && booking.BookingDate.Date <= request.DayEnd.Date
                join customer in _dbContext.Customers on booking.CustomerId equals customer.Id
                join account in _dbContext.Accounts on customer.Account.Id equals account.Id
                join courtSub in _dbContext.CourtSubdivisions on booking.CourtSubdivisionId equals courtSub.Id
                join transaction in _dbContext.Transactions on booking.TransactionId equals transaction.Id
                select new
                {
                    booking,
                    customer,
                    account,
                    courtSub,
                    transaction
                }).ToListAsync();

        var response = dateList.Select((date, index) => new IncomeByBookingResponse
        {
            IdFlag = index + 1,
            DateCheck = date.ToString("yyyy-MM-dd hh:mm:ss"),
            TotalPriceOfDayWasCheckedByAdmin = query
                .Where(q => q.booking.BookingDate.Date == date && (int)q.transaction.AdminCheckStatus == 1)
                .Sum(q => q.booking.TotalAmount),
            TotalPriceOfDayProcessing = query
                .Where(q => q.booking.BookingDate.Date == date && (int)q.transaction.AdminCheckStatus == 0)
                .Sum(q => q.booking.TotalAmount),
            ListBooked = query
                .Where(q => q.booking.BookingDate.Date == date)
                .Select(q => new BookingOfCourtInDayV2
                {
                    BookingId = q.booking.Id,
                    CourtSubdivisionId = q.booking.CourtSubdivisionId,
                    CustomerId = q.booking.CustomerId,
                    AdminCheckStatus = q.booking.Transaction.AdminCheckStatus.ToString(),
                    TransactionId = q.booking.TransactionId,
                    FullNameOfCustomer = q.account.FirstName + " " + q.account.LastName,
                    CourtSubdivisionName = q.courtSub.CourtSubdivisionName,
                    DayTimeBooking = q.booking.BookingDate.ToString("yyyy-MM-dd hh:mm:ss"),
                    TotalPrice = q.booking.TotalAmount,
                })
                .OrderByDescending(x => x.DayTimeBooking)
                .ToList(),
        }).ToList();
        return response;
    }
}