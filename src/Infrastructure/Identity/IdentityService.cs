﻿using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Models.Authentication;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BeatSportsAPI.Domain.Enums;

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
        var user = await _beatSportsDbContext.Accounts
            .FirstOrDefaultAsync(u => u.UserName == loginRequest.Username);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var userRole = user.Role;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
        var secretKey = GetJsonInAppSettingsExtension.GetJson("Jwt:SecretKey");
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("JWT secret key is not configured properly.");
        }

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        string issuer = GetJsonInAppSettingsExtension.GetJson("Jwt:Issuer");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        string audience = GetJsonInAppSettingsExtension.GetJson("Jwt:Audience");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        var claims = new List<Claim>()
        {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, loginRequest.Username),
        new Claim(ClaimTypes.Role, userRole)
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
    /// <summary>
    /// Login Method and Generate AccessToken
    /// </summary>
    /// <param name="loginModelRequest"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<string> AuthenticateAsync(LoginModelRequest loginModelRequest)
    {
        if (string.IsNullOrWhiteSpace(loginModelRequest.Username) || string.IsNullOrWhiteSpace(loginModelRequest.Password))
        {
            throw new ArgumentException("Username and password must be provided.");
        }
        var user = _beatSportsDbContext.Accounts
            .Where(u => u.UserName == loginModelRequest.Username)
            .FirstOrDefault();
        if (user == null)
        {
            throw new NotFoundException("Cannot find this user");
        }
        // Split hashed password into 2 parts: Salt and Hash
        var passwordParts = user.Password.Split(':');
        if (passwordParts.Length != 2)
        {
            throw new FormatException("Password stored is invalid");
        }
        var storedPasswordSalt = Convert.FromBase64String(passwordParts[0]);
        var storedPasswordHash = Convert.FromBase64String(passwordParts[1]);
        // Check password when login 
        Console.WriteLine(storedPasswordSalt);
        Console.WriteLine(storedPasswordHash);
        var isPasswordValid = PasswordHashingExtension.VerifyPasswordHash
            (
                loginModelRequest.Password,
                storedPasswordSalt,
                storedPasswordHash
            );
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Cannot Login");
        }
        var tokenModel = await GenerateToken(new LoginModelRequest
        {
            Username = loginModelRequest.Username,
        });
        return tokenModel.AccessToken;
    }

    /// <summary>
    /// Register new account, hash password
    /// </summary>
    /// <param name="registerModelRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<string> RegisterCustomerAccountAsync(RegisterCustomerModelRequest registerModelRequest, CancellationToken cancellationToken)
    {
        var existedUser = _beatSportsDbContext.Accounts
            .Where(u => u.UserName == registerModelRequest.UserName && u.Email == registerModelRequest.Email)
            .FirstOrDefault();
        if (existedUser != null)
        {
            throw new NotFoundException("This user is existed");
        }
        PasswordHashingExtension.CreatePasswordHashing(
            registerModelRequest.Password,
            out byte[] passwordSalt,
            out byte[] passwordHash
        );
        var passwordSaltString = Convert.ToBase64String(passwordSalt);
        var passwordHashString = Convert.ToBase64String(passwordHash);
        // Combine them into one string separated by a special character (e.g., ':')
        var combinedPassword = $"{passwordSaltString}:{passwordHashString}";
        var newUser = new Account
        {
            UserName = registerModelRequest.UserName,
            Password = combinedPassword,
            Email = registerModelRequest.Email,
            FirstName = registerModelRequest.FirstName,
            LastName = registerModelRequest.LastName,
            DateOfBirth = registerModelRequest.DateOfBirth,
            Gender = registerModelRequest.Gender.ToString(),
            ProfilePictureURL = registerModelRequest.ProfilePictureURL,
            Bio = registerModelRequest.Bio,
            PhoneNumber = registerModelRequest.PhoneNumber,
            Role = RoleEnums.Customer.ToString(),
        };
        await _beatSportsDbContext.Accounts.AddAsync(newUser, cancellationToken);
        var newCustomer = new Customer
        {
            Account = newUser,
            Address = registerModelRequest.Address
        };
        await _beatSportsDbContext.Customers.AddAsync(newCustomer, cancellationToken);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return "Create new account successfully";
    }

    /// <summary>
    /// Create account for owner
    /// </summary>
    /// <param name="registerModelRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<string> RegisterOwnerAccountAsync(RegisterOwnerModelRequest registerModelRequest, CancellationToken cancellationToken)
    {
        var existedUser = _beatSportsDbContext.Accounts
            .Where(u => u.UserName == registerModelRequest.UserName && u.Email == registerModelRequest.Email)
            .FirstOrDefault();
        if (existedUser != null)
        {
            throw new NotFoundException("This user is existed");
        }
        PasswordHashingExtension.CreatePasswordHashing(
            registerModelRequest.Password,
            out byte[] passwordSalt,
            out byte[] passwordHash
        );
        var passwordSaltString = Convert.ToBase64String(passwordSalt);
        var passwordHashString = Convert.ToBase64String(passwordHash);
        // Combine them into one string separated by a special character (e.g., ':')
        var combinedPassword = $"{passwordSaltString}:{passwordHashString}";
        var newUser = new Account
        {
            UserName = registerModelRequest.UserName,
            Password = combinedPassword,
            Email = registerModelRequest.Email,
            FirstName = registerModelRequest.FirstName,
            LastName = registerModelRequest.LastName,
            DateOfBirth = registerModelRequest.DateOfBirth,
            Gender = registerModelRequest.Gender.ToString(),
            ProfilePictureURL = registerModelRequest.ProfilePictureURL,
            Bio = registerModelRequest.Bio,
            PhoneNumber = registerModelRequest.PhoneNumber,
            Role = RoleEnums.Owner.ToString(),
        };
        await _beatSportsDbContext.Accounts.AddAsync(newUser, cancellationToken);
        var newOwner = new Owner
        {
            Account = newUser,
        };
        await _beatSportsDbContext.Owners.AddAsync(newOwner, cancellationToken);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return "Create new account successfully";
    }
}
