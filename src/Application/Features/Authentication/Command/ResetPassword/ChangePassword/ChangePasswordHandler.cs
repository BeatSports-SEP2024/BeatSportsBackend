using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Application.Models.Authentication;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Command.ResetPassword.ChangePassword;
public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public ChangePasswordHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public async Task<BeatSportsResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = _beatSportsDbContext.Accounts
            .Where(a => !a.IsDelete && a.Id == request.AccountId) // Assuming request has a UserId property
            .FirstOrDefault();

        if (user == null)
        {
            return new BeatSportsResponse
            {
                Message = "User not found or account is deleted"
            };
        }

        // Split hashed password into 2 parts: Salt and Hash
        var passwordParts = user.Password.Split(':');
        if (passwordParts.Length != 2)
        {
            throw new FormatException("Password stored is invalid");
        }

        var storedPasswordSalt = Convert.FromBase64String(passwordParts[0]);
        var storedPasswordHash = Convert.FromBase64String(passwordParts[1]);

        // Check password when verify
        var isPasswordValid = PasswordHashingExtension.VerifyPasswordHash(
            request.OldPassword,
            storedPasswordSalt,
            storedPasswordHash
        );

        if (!isPasswordValid)
        {
            return new BeatSportsResponse
            {
                Message = "Invalid old password"
            };
        }

        // Create new password hash and salt
        PasswordHashingExtension.CreatePasswordHashing(request.NewPassword, out byte[] passwordSalt, out byte[] passwordHash);
        
        var passwordSaltString = Convert.ToBase64String(passwordSalt);
        var passwordHashString = Convert.ToBase64String(passwordHash);

        // Combine them into one string separated by a special character (e.g., ':')
        var combinedPassword = $"{passwordSaltString}:{passwordHashString}";

        // Update user's password in the database
        user.Password = combinedPassword;

        _beatSportsDbContext.Accounts.Update(user);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = "Password updated successfully"
        };
    }
}