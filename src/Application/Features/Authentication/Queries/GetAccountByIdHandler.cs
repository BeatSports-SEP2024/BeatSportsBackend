using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using AutoMapper.QueryableExtensions;
using AutoFilterer.Extensions;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class GetAccountByIdHandler : IRequestHandler<GetAccountByIdCommand, AccountResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public GetAccountByIdHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public Task<AccountResponse> Handle(GetAccountByIdCommand request, CancellationToken cancellationToken)
    {
        var isExistedAccount = _beatSportsDbContext.Accounts
            .Where(a => !a.IsDelete)
            .ProjectTo<AccountResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefault();
        if(isExistedAccount == null)
        {
            throw new BadRequestException($"Account with id: {request.AccountId} is not existed");
        }
        return Task.FromResult(isExistedAccount);
    }
}
