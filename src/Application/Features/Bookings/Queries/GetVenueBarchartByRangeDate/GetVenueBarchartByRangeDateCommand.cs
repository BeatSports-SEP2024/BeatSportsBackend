using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetVenueBarchartByRangeDate;
public class GetVenueBarchartByRangeDateCommand : IRequest<List<VenueBarchartResponse>>
{
    public Guid OwnerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class GetVenueBarchartByRangeDateCommandHandler : IRequestHandler<GetVenueBarchartByRangeDateCommand, List<VenueBarchartResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetVenueBarchartByRangeDateCommandHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<List<VenueBarchartResponse>> Handle(GetVenueBarchartByRangeDateCommand request, CancellationToken cancellationToken)
    {
        // check start và end đủ 7 ngày rồi xong sẽ lấy tất cả booking sau
        var startDate = request.StartDate.Date;
        var endDate = request.EndDate.Date;
        if ((endDate - startDate).TotalDays > 6)
        {
            endDate = startDate.AddDays(6);
        }

        // lấy danh sách ngày trong range đưa vào
        var allDateInRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
            .Select(offset => startDate.AddDays(offset)).ToList();

        var bookingExist = await (
                from booking in _dbContext.Bookings
                join subCourt in _dbContext.CourtSubdivisions on booking.CourtSubdivisionId equals subCourt.Id
                join court in _dbContext.Courts on subCourt.CourtId equals court.Id
                where !booking.IsDelete && booking.PlayingDate >= startDate && booking.PlayingDate <= endDate.AddHours(23).AddMinutes(59).AddSeconds(59)
                    && court.OwnerId == request.OwnerId
                group booking by booking.PlayingDate.Date into bookingGroupDate
                select new
                {
                    TotalVenueOfDays = bookingGroupDate.Sum(x => x.TotalAmount),
                    Date = bookingGroupDate.Key
                }
            ).ToListAsync();

        // merge allInDateRange voi boookingExist
        var response = allDateInRange.GroupJoin(
            bookingExist,
            date => date,
            booking => booking.Date,
            (date, bookings) => new VenueBarchartResponse
            {
                Date = date,
                Days = date.DayOfWeek.ToString(),
                TotalVenueOfDays = bookings.Sum(booking => booking.TotalVenueOfDays),
            }).OrderBy(x => x.Date).ToList();
        return response;
    }
}
