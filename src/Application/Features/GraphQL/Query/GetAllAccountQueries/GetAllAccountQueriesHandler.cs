using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Campaigns.Queries.GetAllCampaigns;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.GraphQL.Query.GetAllAccountQueries;
public class GetAllAccountQueriesHandler : IRequestHandler<GetAllAccountQueriesCommand, List<Account>>
{
    private readonly IBeatSportsDbContext _dbContext;

    public GetAllAccountQueriesHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<Account>> Handle(GetAllAccountQueriesCommand request, CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts
                             .Where(a => !a.IsDelete)
                             .Include(x => x.Customer)
                             .ToListAsync();
    }
}
