using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Models.Authentication;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Common.Interfaces;
public interface IIdentityService
{
    Task<string> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<LoginResponse> AuthenticateAsync(LoginModelRequest loginModelRequest);
    Task<string> RegisterCustomerAccountAsync(RegisterCustomerModelRequest registerModelRequest, CancellationToken cancellationToken);
    Task<string> RegisterOwnerAccountAsync(RegisterOwnerModelRequest registerModelRequest, CancellationToken cancellationToken);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
    Task<Result> DeleteUserAsync(string userId);
    public RefreshToken GetRefreshToken(string token);
    Task<LoginResponse> SetNewRefreshTokenAsync(string userId);
}
