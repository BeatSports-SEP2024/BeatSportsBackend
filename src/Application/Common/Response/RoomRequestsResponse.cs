using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.Room;

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
    public Guid LevelId { get; set; }
    public string? Address { get; set; }
    public int? ResquestCount { get; set; }
    public DateTime DatePlaying { get; set; } // Get from Start Time Room
    public decimal? Price { get; set; }
    public string? LevelName { get; set; }
    public DateTime DateRequest { get; set; } // Để phân filter
}