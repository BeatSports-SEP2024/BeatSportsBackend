using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Domain.Common;

namespace BeatSportsAPI.Application.Models.Authentication;
public class RegisterModelRequest : BaseEntity
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string? ProfilePictureURL { get; set; }
    public string? Bio { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Role { get; set; } = null!;
}
