using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Models;
public class NotificationModels
{
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}
