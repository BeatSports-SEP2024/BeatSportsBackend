using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class LoginCommand : IRequest<LoginModelResponse>
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
