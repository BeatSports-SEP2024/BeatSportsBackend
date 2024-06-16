using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Common;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BeatSportsAPI.Application.Models.Authentication;
public class RegisterCustomerModelRequest : IRequest<BeatSportsResponse>
{

    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [EmailAddress]
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    //public DateTime DateOfBirth { get; set; }
    //[EnumDataType(typeof(GenderEnums))]
    //public GenderEnums Gender { get; set; }
    //public string? ProfilePictureURL { get; set; }
    //public IFormFile? ProfilePicture { get; set; }
    //public string? Bio { get; set; }
    public string PhoneNumber { get; set; } = null!;
    //public string Address { get; set; } = null!;
}
