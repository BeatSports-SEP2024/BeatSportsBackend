using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BeatSportsAPI.Application.Features.Authentication.Command.ResetPassword.ResetPasswordByOTP;
public class ResetPasswordByOTPHandler : IRequestHandler<ResetPasswordByOTPCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMemoryCache _memoryCache;

    public ResetPasswordByOTPHandler(IBeatSportsDbContext beatSportsDbContext, IMemoryCache memoryCache)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _memoryCache = memoryCache;
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

    public async Task<BeatSportsResponse> Handle(ResetPasswordByOTPCommand request, CancellationToken cancellationToken)
    {
        var accountResetPassword = _beatSportsDbContext.Accounts
            .Where(a => a.Email == request.userEmail && !a.IsDelete).FirstOrDefault();

        if (accountResetPassword == null) 
        {
            throw new NotFoundException("Cannot Find Your Email");
        }
        //Store OTP in Cache with format: OTP-Email
        var cacheKey = $"OTP-{request.userEmail}";
        var cachedOtp = _memoryCache.Get<string>(cacheKey);

        if(cachedOtp == null || cachedOtp != request.OTP) 
        {
            throw new BadRequestException("OTP is not valid");
        }

        var newPassword = CreatePasswordHash(request.newPassword);
        accountResetPassword.Password = newPassword;
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        //When update password successfully, delete otp
        _memoryCache.Remove(cacheKey);
        return new BeatSportsResponse
        {
            Message = "Update new password successfully"
        };
    }
}