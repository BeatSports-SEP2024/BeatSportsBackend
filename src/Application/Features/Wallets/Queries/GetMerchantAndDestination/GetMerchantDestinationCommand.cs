using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Wallets.Queries.GetMerchantAndDestination;
public class GetMerchantDestinationCommand : IRequest<MerchantNDestinationResponse>
{
}

public class GetMerchantDestinationCommandHandler : IRequestHandler<GetMerchantDestinationCommand, MerchantNDestinationResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetMerchantDestinationCommandHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public async Task<MerchantNDestinationResponse> Handle(GetMerchantDestinationCommand request, CancellationToken cancellationToken)
    {
        var merchantExist = await _beatSportsDbContext.Merchants.Where(m => m.MerchantName == "BeatSports_AppUser").SingleOrDefaultAsync();
        var destinationExist = await _beatSportsDbContext.PaymentsDestinations.ToListAsync();
        var response = new MerchantNDestinationResponse
        {
            MerchantId = merchantExist.Id,
            ListDestination = _mapper.Map<List<DestinationResponse>>(destinationExist)
        };

        return response;
    }
}

public class MerchantNDestinationResponse
{
    public Guid MerchantId { get; set; }
    public List<DestinationResponse> ListDestination { get; set; }
}

public class DestinationResponse : IMapFrom<PaymentDestination>
{
    public Guid Id { get; set; }
    public string? DesName { get; set; } = string.Empty;
    public string? DesShortName { get; set; } = string.Empty;
}
