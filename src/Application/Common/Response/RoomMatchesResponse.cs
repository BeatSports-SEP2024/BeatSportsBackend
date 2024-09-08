using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Domain.Entities.Room;
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
    public Guid CustomerId { get; set; }
    public Guid RoomRequestsId { get; set; }
    public string? CustomerAvatar { get; set; } 
    public string? CustomerName { get; set; }
    public RoomRequestEnums JoinStatus { get; set; }
}

public class RoomMatchesDetailResponse
{
    public Guid RoomMatchId { get; set; }
    public string? RoomName { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerImage { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CourtName { get; set; }
    public string? CourtDescription { get; set; }
    public string[] CourtImage { get; set; }
    public string? Address { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public DateTime PlayingDate { get; set; }
    public string LevelName { get; set; }
    public DateTime StartTimeRoom { get; set; }
    public DateTime EndTimeRoom { get; set; }
    public int MaximumMember { get; set; }
    public int CountMember { get; set; }
    public double PlayingCosts { get; set; }
    public string? RuleRoom { get; set; }
    public string? Note { get; set; }
    public IList<RoomRequestInRoom>? JoiningRequest { get; set; }
    public IList<RoomMemberInRoomResponse>? RoomMembers { get; set; }
    public string? JoinedIfPendingStatus { get; set; }
    public bool? IsPrivate { get; set; }
    public string? MasterName { get; set; }
    public string? MasterAvatar { get; set; }

    public string? SportName { get; set; }
    public string? SportCourtTypeName { get; set; }
    public string? RoomMatchTypeName { get; set; }
    public string? DescriptionRating { get; set; }
    public double? WinRatePercent { get; set; }
    public double? LoseRatePercent { get; set; }
}

public class RoomMemberInRoomResponse
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerImage { get; set; }
    public string RoleInRoom { get; set; }
    public string? Team {  get; set; }
    public string? MatchingResultStatus {  get; set; }
}
