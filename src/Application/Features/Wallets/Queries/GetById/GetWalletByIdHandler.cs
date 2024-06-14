using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Wallets.Queries.GetById;
public class GetWalletByIdHandler : IRequestHandler<GetWalletByIdCommand, WalletResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetWalletByIdHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public async Task<WalletResponse> Handle(GetWalletByIdCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _beatSportsDbContext.Wallets
            .Where(a => a.AccountId == request.AccountId && !a.IsDelete)
            .Select(c => new WalletResponse
            {
                AccountId = c.AccountId,
                Balance = c.Balance,
                WalletId = c.Id,
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (wallet == null)
        {
            throw new NotFoundException($"{request.AccountId} does not have any wallet");
        }

        return wallet;
    }
}