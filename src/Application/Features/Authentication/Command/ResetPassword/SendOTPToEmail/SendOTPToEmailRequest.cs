using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Command.ResetPassword.SendOTPToEmail;
public class SendOTPToEmailRequest : IRequest<BeatSportsResponse>
{
    [Required]
    public string userEmail { get; set; }
}