using System.Data;
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
using System.Security.Cryptography;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Azure;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Authentication.Command.AuthGoogle;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Google.Apis.Auth;
using Newtonsoft.Json;

namespace BeatSportsAPI.Infrastructure.Identity;
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly JwtSettings _jwtSettings;
    private readonly IConfiguration _configuration;
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IConfiguration configuration,
        IOptions<JwtSettings> jwtSettings,
        IBeatSportsDbContext beatSportsDbContext,
        IHttpClientFactory httpClientFactory
        )
    {
        _jwtSettings = jwtSettings.Value;
        _configuration = configuration;
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _beatSportsDbContext = beatSportsDbContext;
        _httpClientFactory = httpClientFactory;
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

    private static string CreatePasswordHash(string password)
    {
        PasswordHashingExtension.CreatePasswordHashing(
            password,
            out byte[] passwordSalt,
            out byte[] passwordHash
        );
        var passwordSaltString = Convert.ToBase64String(passwordSalt);
        var passwordHashString = Convert.ToBase64String(passwordHash);
        // Combine them into one string separated by a special character (e.g., ':')
        var combinedPassword = $"{passwordSaltString}:{passwordHashString}";
        return combinedPassword;
    }

    public async Task<TokenModel> GenerateToken(LoginModelRequest loginRequest)
    {
        var user = await _beatSportsDbContext.Accounts
            .FirstOrDefaultAsync(u => u.UserName == loginRequest.Username);
        var userRole = user.Role;
        JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
        var secretKey = GetJsonInAppSettingsExtension.GetJson("Jwt:SecretKey");
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("JWT secret key is not configured properly.");
        }

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        string? issuer = GetJsonInAppSettingsExtension.GetJson("Jwt:Issuer");
        string? audience = GetJsonInAppSettingsExtension.GetJson("Jwt:Audience");

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

    public async Task<LoginResponse> AuthenticateAsync(LoginModelRequest loginModelRequest)
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

        //create new refeshToken and save to DB
        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken, user, tokenModel);
        var loginResponse = new LoginResponse
        {
            AccessToken = tokenModel.AccessToken,
            RefreshToken = refreshToken
        };
        return loginResponse;
    }

    private RefreshTokenModel GenerateRefreshToken()
    {
        var refreshToken = new RefreshTokenModel
        {
            Token = GenerateRandomAlphanumericString(16),
            Expires = DateTime.Now.AddDays(1)
        };

        return refreshToken;
    }

    private void SetRefreshToken(RefreshTokenModel newRefreshToken, Account user, TokenModel accessToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires,
        };
        
        var refreshToken = _beatSportsDbContext.RefreshToken.Where(x => x.AccountId == user.Id).FirstOrDefault();
        if(refreshToken != null) {
            refreshToken.AccessToken = accessToken.AccessToken;
            refreshToken.Token = newRefreshToken.Token;
            refreshToken.TokenExpires = newRefreshToken.Expires;
            refreshToken.TokenCreated = newRefreshToken.Created;

            _beatSportsDbContext.RefreshToken.Update(refreshToken);
            _beatSportsDbContext.SaveChanges();
        }
        else
        {
            var newRef = new RefreshToken
            {
                AccountId = user.Id,
                AccessToken = accessToken.AccessToken,
                Token = newRefreshToken.Token,
                TokenCreated = newRefreshToken.Created,
                TokenExpires = newRefreshToken.Expires,
            };

            _beatSportsDbContext.RefreshToken.Add(newRef);
            _beatSportsDbContext.SaveChanges(); 
        }
        
    }

    public async Task<LoginResponse> SetNewRefreshTokenAsync(string userId)
    {
        var user = _beatSportsDbContext.Accounts
            .Where(u => u.UserName == userId)
            .FirstOrDefault();
        var loginModel = new LoginModelRequest
        {
            Username = user.UserName
        };

        if (loginModel.Username == null)
        {
            throw new UnauthorizedAccessException("Invalid.");
        }
        var tokenModel = await GenerateToken(new LoginModelRequest
        {
            Username = loginModel.Username,
        });
        //create new refeshToken and save to DB
        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken, user, tokenModel);
        var loginResponse = new LoginResponse
        {
            Message = "Login Successfully",
            AccessToken = tokenModel.AccessToken,
            RefreshToken = refreshToken
        };
        
        return loginResponse;
    }
   

    public RefreshToken GetRefreshToken (string token)
    {
        var refreshToken = _beatSportsDbContext.RefreshToken.Where(x => x.Token.Equals(token)).FirstOrDefault();
        return refreshToken;
    }

    public static string GenerateRandomAlphanumericString(int length)
    {
        const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var stringBuilder = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            var byteBuffer = new byte[sizeof(uint)];

            while (length-- > 0)
            {
                rng.GetBytes(byteBuffer);
                var num = BitConverter.ToUInt32(byteBuffer, 0);
                stringBuilder.Append(validCharacters[(int)(num % (uint)validCharacters.Length)]);
            }
        }
        return stringBuilder.ToString();
    }

    public async Task<string> RegisterCustomerAccountAsync(RegisterCustomerModelRequest registerModelRequest, CancellationToken cancellationToken)
    {
        var existedUser = _beatSportsDbContext.Accounts
            .Where(u => u.UserName == registerModelRequest.UserName && u.Email == registerModelRequest.Email)
            .FirstOrDefault();
        if (existedUser != null)
        {
            throw new NotFoundException("This user is existed");
        }
        var combinedPassword = CreatePasswordHash(registerModelRequest.Password);
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
        var newWallet = new Wallet
        {
            AccountId = newUser.Id,
            Balance = 0,
            Created = DateTime.UtcNow
        };
        await _beatSportsDbContext.Wallets.AddAsync(newWallet, cancellationToken);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return "Create new account successfully";
    }

    public async Task<string> RegisterOwnerAccountAsync(RegisterOwnerModelRequest registerModelRequest, CancellationToken cancellationToken)
    {
        var existedUser = _beatSportsDbContext.Accounts
            .Where(u => u.UserName == registerModelRequest.UserName && u.Email == registerModelRequest.Email)
            .FirstOrDefault();
        if (existedUser != null)
        {
            throw new NotFoundException("This user is existed");
        }
        var combinedPassword = CreatePasswordHash(registerModelRequest.Password);
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
        var newWallet = new Wallet
        {
            AccountId = newUser.Id,
            Balance = 0,
            Created = DateTime.UtcNow
        };
        await _beatSportsDbContext.Wallets.AddAsync(newWallet, cancellationToken);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
        return "Create new account successfully";
    }

    #region Login with goole token
    public async Task<GoogleLoginResponse> GoogleLoginAuthAsync(GoogleLoginRequest request, CancellationToken cancellationToken)
    {
        string idToken = request.IdToken;
        var googleClientIdAndroid = _configuration["Google:ClientIdAndroid"];
        var googleClientIdIOS = _configuration["Google:ClientIdIOS"];
        string platform = request.Platform;
        //var googleClientSecret = _configuration["Google:ClientSecret"];

        string googleClientId = platform.ToLower() == "android" ? googleClientIdAndroid! : googleClientIdIOS!;
        try
        {
            //var tokenResponse = await ExchangeAuthorizationCodeAsync(idToken, googleClientId, googleClientSecret!);

            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleClientId }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);


            var account = await _beatSportsDbContext.Accounts.Where(a => a.GoogleId == payload.Subject).SingleOrDefaultAsync();
            if (account == null)
            {
                PasswordHashingExtension.CreatePasswordHashing(
                        "Password1@",
                        out byte[] passwordSalt,
                        out byte[] passwordHash
                    );
                var passwordSaltString = Convert.ToBase64String(passwordSalt);
                var passwordHashString = Convert.ToBase64String(passwordHash);
                // Combine them into one string separated by a special character (e.g., ':')
                var combinedPassword = $"{passwordSaltString}:{passwordHashString}";
                account = new Account
                {
                    GoogleId = payload.Subject,
                    UserName = payload.Name,
                    Password = combinedPassword,
                    Email = payload.Email,
                    FirstName = payload.FamilyName,
                    LastName = payload.GivenName,
                    DateOfBirth = DateTime.Now,
                    ProfilePictureURL = payload.Picture,
                    Role = RoleEnums.Customer.ToString(),
                };
                await _beatSportsDbContext.Accounts.AddAsync(account, cancellationToken);
                var newCustomer = new Customer
                {
                    Account = account
                };
                await _beatSportsDbContext.Customers.AddAsync(newCustomer, cancellationToken);
                await _beatSportsDbContext.SaveChangesAsync(cancellationToken);
            }
            var token = GenerateJwtGoogleToken(account);
            var response = new GoogleLoginResponse
            {
                AccessToken = token,
                //RefreshToken = tokenResponse,
                User = new UserResponse { Email = account.Email!, 
                    LastName= account.LastName!, 
                    ProfilePictureURL = account.ProfilePictureURL! 
                }
            };

            return response;
        }
        catch (Exception ex)
        {
            throw new BadRequestException($"Invalid Google token {ex.Message}");
        }
    }

    private string GenerateJwtGoogleToken(Account account)
    {
        var user = _beatSportsDbContext.Accounts
            .FirstOrDefault(u => u.UserName == account.UserName);
        var userRole = user.Role;
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
        new Claim(JwtRegisteredClaimNames.Sub, account.UserName),
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

        return tokenString;
    }

    #region refreshToken for google
    private async Task<TokenResponse> ExchangeAuthorizationCodeAsync(string code, string clientId, string clientSecret)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
        var content = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("code", code),
        new KeyValuePair<string, string>("client_id", clientId),
        new KeyValuePair<string, string>("client_secret", clientSecret),
        new KeyValuePair<string, string>("redirect_uri", "https://auth.expo.io/@nhannnn/BeatSports_AppUser"),
        new KeyValuePair<string, string>("grant_type", "authorization_code"),
        });
        request.Content = content;

        var client = _httpClientFactory.CreateClient();
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error exchanging authorization code: {errorContent}");
        }

        var payload = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(payload);

        if (tokenResponse == null)
        {
            throw new Exception($"Failed to deserialize token response: {payload}");
        }
        return tokenResponse;
    }
    #endregion

    #endregion
}
