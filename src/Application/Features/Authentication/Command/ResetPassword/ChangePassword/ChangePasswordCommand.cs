using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Command.ResetPassword.ChangePassword;
public class ChangePasswordCommand : IRequest<BeatSportsResponse>
{
    public Guid AccountId { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmedPassword { get; set; }
}