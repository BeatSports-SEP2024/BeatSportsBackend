using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;

namespace BeatSportsAPI.Application.Common.Response;
//Response for create, update and delete
public class BeatSportsResponse
{
    public string Message { get; set; } = "";
}

public class RoomMatchResponse
{
    public string Message { get; set; } = "";
    public string? RoomMatchId { get; set; }
}

public class BookingSuccessResponse
{
    public Guid BookingId { get; set; }
    public string Message { get; set; } = "";
}