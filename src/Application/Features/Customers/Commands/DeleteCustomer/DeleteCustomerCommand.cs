using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Customers.Commands.DeleteCustomer;
public class DeleteCustomerCommand : IRequest<BeatSportsResponse>
{
    public Guid CustomerId { get; set; }
}
