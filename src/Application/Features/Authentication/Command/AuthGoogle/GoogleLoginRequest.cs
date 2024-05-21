using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Command.AuthGoogle;
public class GoogleLoginRequest : IRequest<GoogleLoginResponse>
{
    public string IdToken { get; set; }
    public string Platform { get; set; }
}
