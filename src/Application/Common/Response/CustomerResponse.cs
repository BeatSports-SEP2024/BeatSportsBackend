using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Common.Response;
public class CustomerResponse : IMapFrom<Customer>
{
    public Guid CustomerId { get; set; }
    public Guid AccountId { get; set; }
    public int RewardPoints { get; set; } = 0;
    public string? Address { get; set; }
}
