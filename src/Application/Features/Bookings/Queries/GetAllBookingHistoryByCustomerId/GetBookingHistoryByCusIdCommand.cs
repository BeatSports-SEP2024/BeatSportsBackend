using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetAllBookingHistoryByCustomerId;
public class GetBookingHistoryByCusIdCommand : IRequest<List<BookingHistoryByCustomerId>>
{
    public Guid CustomerId { get; set; }
}

public class GetBookingHistoryByCusIdCommandHandler : IRequestHandler<GetBookingHistoryByCusIdCommand, List<BookingHistoryByCustomerId>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetBookingHistoryByCusIdCommandHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<BookingHistoryByCustomerId>> Handle(GetBookingHistoryByCusIdCommand request, CancellationToken cancellationToken)
    {
        var listBookingExist = await (
            from booking in _dbContext.Bookings
            where !booking.IsDelete && booking.CustomerId == request.CustomerId
            join customer in _dbContext.Customers on booking.CustomerId equals customer.Id
            join subCourt in _dbContext.CourtSubdivisions on booking.CourtSubdivisionId equals subCourt.Id
            join court in _dbContext.Courts on subCourt.CourtId equals court.Id
            join feedback in _dbContext.Feedbacks on booking.Id equals feedback.BookingId into feedbackJoin
            from feedback in feedbackJoin.DefaultIfEmpty()
            select new BookingHistoryByCustomerId
            {
                BookingId = booking.Id,
                CustomerId = customer.Id,
                CampaignId = booking.CampaignId,
                CourtSubdivisionId = subCourt.Id,
                CourtSubName = subCourt.CourtSubdivisionName,
                CourtName = court.CourtName,
                CourtAddress = court.Address,
                CourtImage = court.WallpaperUrls,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                IsRoomBooking = booking.IsRoomBooking,
                IsDeposit = booking.IsDeposit,
                PlayingDate = booking.PlayingDate,
                StartTimePlaying = booking.StartTimePlaying,
                EndTimePlaying = booking.EndTimePlaying,
                BookingStatus = booking.BookingStatus,
                FeedbackId = feedback.Id,

            }).ToListAsync();
        return listBookingExist;
    }
}


