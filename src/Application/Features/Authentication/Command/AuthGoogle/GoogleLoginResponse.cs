using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;
using Newtonsoft.Json;

namespace BeatSportsAPI.Application.Features.Authentication.Command.AuthGoogle;
public class GoogleLoginResponse
{
    public string? AccessToken { get; set; } 
    //public TokenResponse? RefreshToken { get; set; }
    public UserResponse? User { get; set; }
}

public class TokenResponse
{
    [JsonProperty("id_token")]
    public string IdToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
}

public class UserResponse
{
    public string Email { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfilePictureURL { get; set; } = string.Empty;
}
