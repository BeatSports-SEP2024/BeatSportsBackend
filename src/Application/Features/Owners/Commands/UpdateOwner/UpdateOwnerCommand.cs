using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Owners.Commands.UpdateOwner;
public class UpdateOwnerCommand : IRequest<BeatSportsResponse>
{
    public Guid OwnerId { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    //public string? FirstName { get; set; }
    //public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    //[EnumDataType(typeof(GenderEnums))]
    //public GenderEnums Gender { get; set; }
    public string? ProfilePictureURL { get; set; }
    public string? Bio { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; }
}
