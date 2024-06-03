using System.ComponentModel.DataAnnotations;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Queries;
public class GetAllBookingCommand : IRequest<PaginatedList<BookingResponse>>
{
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
}