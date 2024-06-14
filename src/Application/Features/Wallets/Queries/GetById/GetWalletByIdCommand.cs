using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Wallets.Queries.GetById;
public class GetWalletByIdCommand : IRequest<WalletResponse>
{
    public Guid AccountId { get; set; }
}