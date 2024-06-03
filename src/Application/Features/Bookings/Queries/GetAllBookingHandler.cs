using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Queries;
public class GetAllBookingHandler : IRequestHandler<GetAllBookingCommand, PaginatedList<BookingResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllBookingHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<BookingResponse>> Handle(GetAllBookingCommand request, CancellationToken cancellationToken)
    {
        var bookingList = _beatSportsDbContext.Bookings
            .Where(b => !b.IsDelete)
            .ProjectTo<BookingResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize); 

        return bookingList;
    }
}
