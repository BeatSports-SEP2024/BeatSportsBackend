using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Models.Authentication;
using BeatSportsAPI.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BeatSportsAPI.Infrastructure.Identity;
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly JwtSettings _jwtSettings;
    private readonly IConfiguration _configuration;
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IConfiguration configuration,
        IOptions<JwtSettings> jwtSettings,
        IBeatSportsDbContext beatSportsDbContext
        )
    {
        _jwtSettings = jwtSettings.Value;
        _configuration = configuration;
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<string> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<TokenModel> GenerateToken(LoginModelRequest loginRequest)
    {
        JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
        var secretKey = GetJsonInAppSettingsExtension.GetJson("Jwt:SecretKey");
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("JWT secret key is not configured properly.");
        }

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        string issuer = GetJsonInAppSettingsExtension.GetJson("Jwt:Issuer");
        string audience = GetJsonInAppSettingsExtension.GetJson("Jwt:Audience");

        var claims = new List<Claim>()
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, loginRequest.Username)
    };

        var expiry = DateTime.UtcNow.AddMinutes(30);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expiry,
            SigningCredentials = credentials
        };

        var token = jwtHandler.CreateToken(tokenDescriptor);
        string tokenString = jwtHandler.WriteToken(token);

        return new TokenModel { AccessToken = tokenString };
    }

    public async Task<string> AuthenticateAsync(LoginModelRequest loginModelRequest)
    {
        if (string.IsNullOrWhiteSpace(loginModelRequest.Username) || string.IsNullOrWhiteSpace(loginModelRequest.Password))
        {
            throw new ArgumentException("Username and password must be provided.");
        }

        //var user = await _userManager.FindByNameAsync(loginModelRequest.Username);
        //if (user == null)
        //{
        //    // Throw more specific exception or handle this scenario appropriately
        //    throw new KeyNotFoundException("User does not exist.");
        //}

        //var validPassword = await _userManager.CheckPasswordAsync(user, loginModelRequest.Password);
        //if (!validPassword)
        //{
        //    // Consider throwing an exception or handling this case specifically
        //    throw new UnauthorizedAccessException("Password is incorrect.");
        //}

        //// User is authenticated; generate token
        //var tokenModel = await GenerateToken(new LoginModelRequest
        //{
        //    Username = user.UserName,
        //});
        var user = _beatSportsDbContext.Accounts
            .Where(u => u.UserName == loginModelRequest.Username && u.Password == loginModelRequest.Password);
        if (user == null)
        {
            throw new NotFoundException("Cannot find this user");
        }
        var tokenModel = await GenerateToken(new LoginModelRequest
        {
            Username = loginModelRequest.Username,
        });
        return tokenModel.AccessToken;
    }
}
