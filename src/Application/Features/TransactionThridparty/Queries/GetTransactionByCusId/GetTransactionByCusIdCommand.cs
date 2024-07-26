using System.Dynamic;
using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BeatSportsAPI.Application.Features.TransactionThridparty.Queries.GetTransactionByCusId;
public class GetTransactionByCusIdCommand : IRequest<List<TransactionThirdpartyResponse>>
{
    public Guid CustomerId { get; set; }
}

public class GetTransactionByCusIdCommandHandler : IRequestHandler<GetTransactionByCusIdCommand, List<TransactionThirdpartyResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetTransactionByCusIdCommandHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public async Task<List<TransactionThirdpartyResponse>> Handle(GetTransactionByCusIdCommand request, CancellationToken cancellationToken)
    {
        var listTransaction = new List<TransactionThirdpartyResponse>();
        var cusIdExist = await _beatSportsDbContext.Customers.Where(c => c.Id == request.CustomerId).FirstOrDefaultAsync();
        var paymentExist = await _beatSportsDbContext.Payments.Where(p => p.AccountId == cusIdExist!.AccountId).ToListAsync();
        foreach (var payment in paymentExist)
        {
            var transactionCusIdExist = await _beatSportsDbContext.PaymentTransactions.Where(pt => pt.PaymentId == payment.Id).FirstOrDefaultAsync();


            //var transactionPayload = JsonConvert.DeserializeObject<TransactionPayload>(transactionCusIdExist.TranPayload!);
            //JObject transactionPayload = JObject.Parse(transactionCusIdExist!.TranPayload!);
            var transactionPayload = JsonConvert.DeserializeObject<ExpandoObject>(transactionCusIdExist!.TranPayload!)!;

            var data = new TransactionThirdpartyResponse
            {
                TransactionId = transactionCusIdExist.Id,
                TransactionMessage = transactionCusIdExist!.TranMessage,
                TransactionPayload = transactionPayload,
                TransactionStatus = transactionCusIdExist.TranStatus,
                TransactionAmount = transactionCusIdExist.TranAmount,
                TransactionDate = transactionCusIdExist.TranDate,
                PaymentId = transactionCusIdExist.PaymentId,
                TransactionType = payment.PaymentType,
            };
            listTransaction.Add(data);
        }
        return listTransaction.OrderByDescending(p => p.TransactionDate).ToList();
    }
}
