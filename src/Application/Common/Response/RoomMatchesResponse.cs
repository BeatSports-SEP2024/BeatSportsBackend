using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Response;
public class RoomMatchesResponse
{
    public Guid RoomMatchId { get; set; }
    public Guid LevelId { get; set; }
    public Guid BookingId { get; set; }
    public string RoomName { get; set; }
    public DateTime StartTimeRoom { get; set; }
    public DateTime EndTimeRoom { get; set; }
    public int MaximumMember { get; set; }
    public string? RuleRoom { get; set; }
    public string? Note { get; set; }
}

public class GetRoomRequestInRoom
{
    public Guid RoomMatchId { get; set; }
    public List<RoomRequestInRoom>? JoiningRequest { get; set; }
}

public class RoomRequestInRoom
{
    public Guid AccountId { get; set; }
    public Guid RoomRequestsId { get; set; }
    public string? CustomerAvatar { get; set; } 
    public string? CustomerName { get; set; }
    public RoomRequestEnums JoinStatus { get; set; }
}