using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Customers.Queries;
public class GetCustomerByIdCommand : IRequest<CustomerResponse>
{
    [Required]
    public Guid CustomerId { get; set; }
}
