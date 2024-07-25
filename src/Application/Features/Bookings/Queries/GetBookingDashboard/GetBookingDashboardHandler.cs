using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDashboard;

public class BookingDashboardResult
{
    public string Type { get; set; }
    public int AllBooking { get; set; }
    public Dictionary<string, BookingDetails> Items { get; set; }
}

public class BookingDetails
{
    public int Count { get; set; }
    public List<BookingDashboard> BookingList { get; set; }
}

public class GetBookingDashboardHandler : IRequestHandler<GetBookingDashboardCommand, BookingDashboardResult>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetBookingDashboardHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BookingDashboardResult> Handle(GetBookingDashboardCommand request, CancellationToken cancellationToken)
    {
        var query = _beatSportsDbContext.Bookings
            .Include(b => b.CourtSubdivision)
            .ThenInclude(cs => cs.Court)
            .Where(b => !b.IsDelete && !b.BookingStatus.Equals(BookingEnums.Cancel.ToString()));

        var result = new BookingDashboardResult
        {
            Type = request.BookingFilter.ToString().ToLower(),
            Items = new Dictionary<string, BookingDetails>()
        };

        switch (request.BookingFilter.ToString())
        {
            case "Hours":
                for (int hour = 0; hour < 24; hour += 2)
                {
                    var bookingsInGroup = query
                        .Where(b => b.Created.Hour >= hour && b.Created.Hour < hour + 2)
                        .ToList();

                    result.Items[$"{hour:00}-{hour + 2:00}"] = new BookingDetails
                    {
                        Count = bookingsInGroup.Count,
                        BookingList = bookingsInGroup.Select(BookingDashboards).ToList()
                    };
                }
                break;

            case "Days":
                var allBookings = query.ToList(); // Lấy tất cả các bản ghi vào bộ nhớ
                for (int day = 0; day < 7; day++)
                {
                    var bookingsInDay = allBookings
                        .Where(b => (int)b.Created.DayOfWeek == day)
                        .ToList();

                    result.Items[Enum.GetName(typeof(DayOfWeek), day)] = new BookingDetails
                    {
                        Count = bookingsInDay.Count,
                        BookingList = bookingsInDay.Select(BookingDashboards).ToList()
                    };
                }
                break;

            case "Months":
                for (int month = 1; month <= 12; month++)
                {
                    var bookingsInMonth = query.Where(b => b.Created.Month == month).ToList();
                    result.Items[$"Month {month}"] = new BookingDetails
                    {
                        Count = bookingsInMonth.Count,
                        BookingList = bookingsInMonth.Select(BookingDashboards).ToList()
                    };
                }
                break;

            default:
                throw new BadRequestException("Invalid filter option");
        }

        result.AllBooking = result.Items.Sum(i => i.Value.Count);
        return result;
    }

    private BookingDashboard BookingDashboards(Booking b)
    {
        return new BookingDashboard
        {
            BookingId = b.Id,
            CustomerId = b.CustomerId,
            CampaignId = b.CampaignId,
            CourtSubdivisionId = b.CourtSubdivisionId,
            CourtName = b.CourtSubdivision?.Court?.CourtName ?? "Unknown Court",
            CourtSubName = b.CourtSubdivision?.CourtSubdivisionName ?? "Unknown Subdivision",
            BookingDate = b.BookingDate,
            TotalAmount = b.TotalAmount,
            IsRoomBooking = b.IsRoomBooking,
            IsDeposit = b.IsDeposit,
            PlayingDate = b.PlayingDate,
            StartTimePlaying = b.StartTimePlaying,
            EndTimePlaying = b.EndTimePlaying,
            BookingStatus = b.BookingStatus,
        };
    }
}