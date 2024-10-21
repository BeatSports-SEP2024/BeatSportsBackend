using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Owners.Commands.DeleteOwner;
using MediatR;

namespace BeatSportsAPI.Application.Features.Customers.Commands.DeleteCustomer;
public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;

    public DeleteCustomerHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponse> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        //check Customer
        var customer = _dbContext.Customers.Where(x => x.Id == request.CustomerId).SingleOrDefault();
        if (customer == null || customer.IsDelete)
        {
            throw new BadRequestException($"Customer with Customer ID:{request.CustomerId} does not exist or have been delele");
        }
        customer.IsDelete = true;
        _dbContext.Customers.Update(customer);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Delete Customer successfully!"
        });
    }
}
