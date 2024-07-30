using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingByCourtId;
public class GetBookingByCourtIdHandler : IRequestHandler<GetBookingByCourtIdCommand, List<GetBookingByCourtIdResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetBookingByCourtIdHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<List<GetBookingByCourtIdResponse>> Handle(GetBookingByCourtIdCommand request, CancellationToken cancellationToken)
    {
        var query = await
            (
                from booking in _beatSportsDbContext.Bookings 
                where !booking.IsDelete && booking.CourtSubdivision.Court.Id == request.CourtId
                join customer in _beatSportsDbContext.Customers on booking.CustomerId equals customer.Id
                join account in _beatSportsDbContext.Accounts on customer.Account.Id equals account.Id
                join courtSub in _beatSportsDbContext.CourtSubdivisions on booking.CourtSubdivisionId equals courtSub.Id
                select new GetBookingByCourtIdResponse
                {
                    CourtId = courtSub.Court.Id,
                    BookingId = booking.Id,
                    BookingDate = booking.BookingDate,
                    CourtAddress = courtSub.Court.Address,
                    CourtName = courtSub.Court.CourtName,
                    CustomerAddress = customer.Address,
                    TotalAmount = booking.TotalAmount,
                    CustomerBookName = customer.Account.FirstName + " " + customer.Account.LastName,
                    CustomerId = customer.Id,
                    StatusBooking = booking.BookingStatus,
                }
            ).ToListAsync();
        return query;
    }
}
