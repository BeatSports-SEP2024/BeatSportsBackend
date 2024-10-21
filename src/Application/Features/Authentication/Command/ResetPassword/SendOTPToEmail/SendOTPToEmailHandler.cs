using System.Net;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BeatSportsAPI.Application.Features.Authentication.Command.ResetPassword.SendOTPToEmail;
public class SendOTPToEmailHandler : IRequestHandler<SendOTPToEmailRequest, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _memoryCache;

    public SendOTPToEmailHandler(IBeatSportsDbContext beatSportsDbContext,
        IEmailService emailService,
        IMemoryCache memoryCache)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _emailService = emailService;
        _memoryCache = memoryCache;
    }

    public async Task<BeatSportsResponse> Handle(SendOTPToEmailRequest request, CancellationToken cancellationToken)
    {
        var isEmailValid = _beatSportsDbContext.Accounts
            .Where(a => a.Email == request.userEmail && !a.IsDelete)
            .SingleOrDefault();

        if (isEmailValid == null)
        {
            throw new NotFoundException("Cannot Find Your Email");
        }

        var otp = new Random().Next(100000, 999999).ToString();
        //Expired after 5 minutes
        var expiredTime = DateTime.UtcNow.AddMinutes(5);

        // Store OTP in cache
        _memoryCache.Set($"OTP-{request.userEmail}", otp, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
        await _emailService.SendEmailAsync(request.userEmail, "Mã OTP của bạn", $"Mã OTP của bạn là: {otp} và có hiệu lực trong vòng 5 phút");
        return new BeatSportsResponse
        {
            Message = "OTP is sent successfully"
        };
    }
}