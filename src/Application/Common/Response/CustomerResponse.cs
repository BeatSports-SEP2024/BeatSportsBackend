using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Common.Response;
public class CustomerResponse : IMapFrom<Customer>
{
    public Guid CustomerId { get; set; }
    public Guid AccountId { get; set; }
    public int RewardPoints { get; set; } = 0;
    public string? Address { get; set; }
    public string UserName { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string? ProfilePictureURL { get; set; }
    public string? Bio { get; set; }
    public string PhoneNumber { get; set; }

}
