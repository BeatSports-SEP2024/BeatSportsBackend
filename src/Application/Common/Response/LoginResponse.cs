using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Models.Authentication;

namespace BeatSportsAPI.Application.Common.Response;
public class LoginResponse
{
    public string Message { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public RefreshTokenModel RefreshToken { get; set; }
}
