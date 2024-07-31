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
        throw new NotImplementedException();
    }
}
