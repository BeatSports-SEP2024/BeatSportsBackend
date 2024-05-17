using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Models.Authentication;
public class LoginModelRequest : IRequest<LoginResponse>
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
