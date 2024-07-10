using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Owners.Commands.UpdateOwner;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Customers.Commands.UpdateCustomer;
public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public UpdateCustomerHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Check Customer
        var customer = _dbContext.Customers.Where(x => x.Id == request.CustomerId)
                                     .Include(x => x.Account)
                                     .SingleOrDefault();

        if (customer == null || customer.IsDelete)
        {
            throw new BadRequestException($"Customer with Customer ID:{request.CustomerId} does not exist or have been delele");
        }

        customer.Account.Email = request.Email;
        //customer.Account.FirstName = request.FirstName;
        //customer.Account.LastName = request.LastName;
        customer.Account.DateOfBirth = request.DateOfBirth;
        //customer.Account.Gender = request.Gender.ToString();
        customer.Account.PhoneNumber = request.PhoneNumber;
        customer.Account.ProfilePictureURL = request.ProfilePictureURL;
        customer.Account.Bio = request.Bio;
        customer.Address = request.Address;

        _dbContext.Customers.Update(customer);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Update Customer successfully!"
        });
    }
}
