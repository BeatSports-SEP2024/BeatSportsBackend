using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Domain.Entities;
using Google;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers.Queries;

public class QueryDatas
{
    //demo Account
    public List<Account> GetAccounts([Service] IBeatSportsDbContext dbContext)
    {
        return dbContext.Accounts
            .Include(x => x.Customer)
            .ThenInclude(x => x.Bookings)
            .ThenInclude(x => x.CourtSubdivision)
            .ThenInclude(x => x.Court)
            .ThenInclude(x => x.Owner)
            .ToList();

    }

    public List<Campaign> GetCampaigns([Service] IBeatSportsDbContext dbContext)
    {
        return dbContext.Campaigns.ToList();
    }
}
