using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Attributes;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Common;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Models.Authentication;
public class RegisterOwnerModelRequest : IRequest<BeatSportsResponse>
{
    [Required]
    [Normalize]
    public string UserName { get; set; } = null!;
    [EmailAddress]
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }   
    public string? Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    [EnumDataType(typeof(GenderEnums))]
    public GenderEnums Gender { get; set; }
    public string PhoneNumber { get; set; } = null!;
}
