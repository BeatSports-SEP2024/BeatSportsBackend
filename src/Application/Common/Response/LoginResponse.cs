using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Models.Authentication;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Common.Response;
public class LoginResponse
{
    public string Message { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; }
    public AccountResponseForLogin UserInfo { get; set; }
}

public class AccountResponseForLogin
{
    public Guid? Id { get; set; } = null;
    public Guid AccountId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public Guid WalletId { get; set; }
    public string Role { get; set; } 
}
