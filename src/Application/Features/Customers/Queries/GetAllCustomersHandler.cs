using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Customers.Queries;
public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersCommand, PaginatedList<CustomerResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;
    public GetAllCustomersHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }
    public async Task<PaginatedList<CustomerResponse>> Handle(GetAllCustomersCommand request, CancellationToken cancellationToken)
    {
        if (request.PageIndex <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page index and page size cannot less than 0");
        }

        IQueryable<Customer> query = _beatSportsDbContext.Customers
            .Where(x => !x.IsDelete)
            .OrderByDescending(b => b.Created)
            .Include(c => c.Account)
                .ThenInclude(a => a.Wallet);

        var list = query.Select(c => new CustomerResponse
        {
            AccountId = c.AccountId,
            WalletId = c.Account.Wallet.Id,
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
            RewardPoints = c.RewardPoints,
            Created = c.Created
        })
        .PaginatedListAsync(request.PageIndex, request.PageSize);

        return await list;
    }
}
