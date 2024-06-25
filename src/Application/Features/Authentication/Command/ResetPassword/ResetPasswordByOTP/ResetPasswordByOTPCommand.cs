using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Command.ResetPassword.ResetPasswordByOTP;
public class ResetPasswordByOTPCommand : IRequest<BeatSportsResponse>
{
    public string userEmail { get; set; }
    public string OTP { get; set; }
    public string newPassword { get; set; }
}