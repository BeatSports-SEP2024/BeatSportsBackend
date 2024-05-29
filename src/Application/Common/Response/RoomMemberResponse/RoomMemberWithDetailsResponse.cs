using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.Room;

namespace BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
public class RoomMemberWithDetailsResponse 
{
    public Guid RoomMemberId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
    public string? RoleInRoom { get; set; }
    public string? ProfilePictureURL { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}