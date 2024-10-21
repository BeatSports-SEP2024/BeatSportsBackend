using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Features.Customers.Queries;
public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdCommand, CustomerResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetCustomerByIdHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<CustomerResponse> Handle(GetCustomerByIdCommand request, CancellationToken cancellationToken)
    {
        IQueryable<Customer> query = _beatSportsDbContext.Customers
             .Where(x => x.Id == request.CustomerId && !x.IsDelete)
             .Include(c => c.Account);

        var customer = query.Select(c => new CustomerResponse
        {
            AccountId = c.AccountId,
            CustomerId = c.Id,
            UserName = c.Account.UserName,
            Email = c.Account.Email,
            FirstName = c.Account.FirstName,
            LastName = c.Account.LastName,
            DateOfBirth = c.Account.DateOfBirth,
            Gender = c.Account.Gender,
            ProfilePictureURL = c.Account.ProfilePictureURL,
            Bio = c.Account.Bio,
            PhoneNumber = c.Account.PhoneNumber,
            Address = c.Address,
            RewardPoints = c.RewardPoints

        }).SingleOrDefault();

        if (customer == null)
        {
            throw new NotFoundException($"Do not find customer with customer ID: {request.CustomerId}");
        }
        return Task.FromResult(customer);
    }
}
