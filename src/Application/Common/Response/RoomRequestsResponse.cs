using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Response;
public class RoomRequestsResponse : IMapFrom<RoomRequest>
{
    public Guid RoomRequestId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
    public string JoiningStatus { get; set; }
    public DateTime DateRequest { get; set; }
    public DateTime DateApprove { get; set; }
}

public class RoomRequestResponseForCustomer
{
    public string? MasterName { get; set; }
    public Guid RoomRequestId { get; set; }
    public string? RoomName { get; set; }
    public string? CourtName { get; set; }
    public Guid LevelId { get; set; }
    public string? Address { get; set; }
    public int? NumberOfMember { get; set; }
    public DateTime DatePlaying { get; set; } // Get from Start Time Room
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public decimal? Price { get; set; }
    public string? LevelName { get; set; }
    public DateTime DateRequest { get; set; } // Để phân filter
    public int? MaxMember { get; set; }
    public Guid? RoomMatchId { get; set; }
}

public class PublicRoomResponse
{
    public string? MasterName { get; set; }
    //public Guid RoomRequestId { get; set; }
    public Guid LevelId { get; set; }
    public string? Address { get; set; }
    public string? RoomName { get; set; }
    public string? CourtName { get; set; }
    public int? NumberOfMember { get; set; }
    public DateTime DatePlaying { get; set; } // Get from Start Time Room
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public decimal? Price { get; set; }
    public string? LevelName { get; set; }
    //public DateTime DateRequest { get; set; } // Để phân filter
    public int? MaxMember { get; set; }
    public Guid? RoomMatchId { get; set; }
}

public class JoinListResponse
{
    public string? MasterName { get; set; }
    public string? RoomName { get; set; }
    public string? CourtName { get; set; }
    public Guid LevelId { get; set; }
    public string? Address { get; set; }
    public int? NumberOfMember { get; set; }
    public DateTime DatePlaying { get; set; } // Get from Start Time Room
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public decimal? Price { get; set; }
    public string? LevelName { get; set; }
    public int? MaxMember { get; set; }
    public Guid? RoomMatchId { get; set; }
}
public class RoomRequestsResponseForGetAll
{
    public List<RoomRequestResponseForCustomer>? PendingRoomList { get; set; }
    public List<JoinListResponse>? JoinedRoomList { get; set; }
    public List<PublicRoomResponse>? PublicRoomList { get; set; }
}