using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetAllBookingsByCustomerId;
public class GetAllBookingsByCustomerIdHandler : IRequestHandler<GetAllBookingsByCustomerIdCommand, PaginatedList<BookingByCustomerId>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllBookingsByCustomerIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public Task<PaginatedList<BookingByCustomerId>> Handle(GetAllBookingsByCustomerIdCommand request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Bookings
            .Where(b => !b.IsDelete && b.IsRoomBooking == false);

        switch (request.BookingFilter.ToString())
        {
            case "Approved":
                query = query.Where(c => c.BookingStatus == BookingEnums.Approved.ToString());
                break;

            case "Finished":
                query = query.Where(c => c.BookingStatus == BookingEnums.Finished.ToString());
                break;

            case "Cancel":
                query = query.Where(c => c.BookingStatus == BookingEnums.Cancel.ToString());
                break;

            default:
                throw new BadRequestException("An error is occured");
        }

        query = query.OrderByDescending(b => b.Created);

        var list = query.Select(c => new BookingByCustomerId
        {
            BookingId = c.Id,
            CustomerId = c.CustomerId,
            CampaignId = c.CampaignId,
            CourtSubdivisionId = c.CourtSubdivisionId,
            CourtName = c.CourtSubdivision.Court.CourtName,
            CourtSubName = c.CourtSubdivision.CourtSubdivisionName,
            BookingDate = c.BookingDate,
            TotalAmount = c.TotalAmount,
            IsRoomBooking = c.IsRoomBooking,
            IsDeposit = c.IsDeposit,
            PlayingDate = c.PlayingDate,
            StartTimePlaying = c.StartTimePlaying,
            EndTimePlaying = c.EndTimePlaying,
            BookingStatus = c.BookingStatus,
        }).PaginatedListAsync(request.PageIndex, request.PageSize);

        return list;
    }
}