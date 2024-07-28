using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailByCustomer;
public class GetBookingDetailByCustomerCommand : IRequest<List<BookingDetailByCustomer>>
{
    public Guid CustomerId { get; set; }
}