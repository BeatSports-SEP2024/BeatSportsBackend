using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, BeatSportsResponse>
{
    private readonly IMapper _mapper;
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public RegisterCommandHandler(IMapper mapper, IBeatSportsDbContext beatSportsDbContext)
    {
        _mapper = mapper;
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var checkedUser = _beatSportsDbContext.Accounts
            .Where(u => u.UserName == request.UserName)
            .FirstOrDefault();

        if (checkedUser != null)
        {
            throw new Exception("User already exists");
        }

        byte[] passwordHash, passwordSalt;
        PasswordHashingExtension.CreatePasswordHashing(
            request.Password,
            out passwordSalt,
            out passwordHash
        );

        var combinedPassword = $"{Convert.ToBase64String(passwordSalt)}:{Convert.ToBase64String(passwordHash)}";

        var newUser = new Account
        {
            UserName = request.UserName,
            Password = combinedPassword,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            ProfilePictureURL = request.ProfilePictureURL,
            Bio = request.Bio,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role
        };

        await _beatSportsDbContext.Accounts.AddAsync(newUser, cancellationToken);
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = "User registered successfully"
        };
    }
}
