using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionByCustomerV1;
public class GetAllTransactionByCustomerV1Query : IRequest<List<TransactionResponse>>
{
    public Guid CustomerId { get; set; }
}