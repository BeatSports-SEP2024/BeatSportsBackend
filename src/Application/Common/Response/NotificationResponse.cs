using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Response;
public class NotificationResponse
{
    public Guid NotificationId { get; set; }
    public Guid AccountId { get; set; }
    public string BookingId { get; set; }
    public string RoomMatchId { get; set; }
    public string FullName { get; set; }
    public string AccountImage { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
    public bool IsRead { get; set; }
    public string Type { get; set; }
}
