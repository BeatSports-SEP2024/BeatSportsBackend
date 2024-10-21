using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities;
public class Admin : BaseAuditableEntity
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
}
