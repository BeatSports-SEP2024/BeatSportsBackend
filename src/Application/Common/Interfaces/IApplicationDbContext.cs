using BeatSportsAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Common.Interfaces;
public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
