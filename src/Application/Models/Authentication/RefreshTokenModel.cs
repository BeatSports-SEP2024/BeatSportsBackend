using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Models.Authentication;
public class RefreshTokenModel
{
    public required string Token { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Expires { get; set; }
}
