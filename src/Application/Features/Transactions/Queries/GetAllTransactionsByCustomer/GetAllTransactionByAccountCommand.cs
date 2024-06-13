using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionsByCustomer;
public class GetAllTransactionByAccountCommand : IRequest<PaginatedList<TransactionResponse>>
{
    public Guid AccountId { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}