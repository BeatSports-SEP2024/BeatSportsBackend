using Azure.Core;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.GraphQL.Query.GetAllAccountQueries;
using BeatSportsAPI.Domain.Entities;
using Google;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers.Queries;

public class QueryDatas
{
    //Get Accounts
    public async Task<IEnumerable<Account>> GetAccounts([Service] IMediator mediator, CancellationToken token) =>
        await mediator.Send(new GetAllAccountQueriesCommand(), token);

}
