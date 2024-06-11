using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

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

    public Task<WalletResponse> Handle(GetWalletByIdCommand request, CancellationToken cancellationToken)
    {
        var wallet = _beatSportsDbContext.Wallets
            .Where(w => w.Id == request.WalletId && !w.IsDelete)
            .ProjectTo<WalletResponse>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
        if(wallet == null)
        {
            throw new NotFoundException($"{request.WalletId} does not existed");
        }
        return Task.FromResult(wallet);
    }
}
