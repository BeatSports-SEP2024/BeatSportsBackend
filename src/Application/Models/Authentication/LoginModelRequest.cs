using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Models.Authentication;
public class LoginModelRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
