using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
