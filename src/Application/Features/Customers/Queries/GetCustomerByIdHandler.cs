using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;

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
        var isExistedCustomer = _beatSportsDbContext.Customers
            .Where(c => c.Id == request.CustomerId && !c.IsDelete)
            .ProjectTo<CustomerResponse>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
        if(isExistedCustomer == null)
        {
            throw new NotFoundException($"{request.CustomerId} not existed");
        }
        return Task.FromResult(isExistedCustomer);
    }
}
