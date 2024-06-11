using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Wallets.Queries;
public class GetAllWalletsHandler : IRequestHandler<GetAllWalletCommand, PaginatedList<WalletResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllWalletsHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<WalletResponse>> Handle(GetAllWalletCommand request, CancellationToken cancellationToken)
    {
        var wallets = _beatSportsDbContext.Wallets
            .Where(w => !w.IsDelete)
            .ProjectTo<WalletResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageIndex, request.PageSize);
        return wallets;
    }
}