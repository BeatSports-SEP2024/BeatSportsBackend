using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Domain.Entities;
using MediatR;

namespace BeatSportsAPI.Application.Features.GraphQL.Query.GetAllAccountQueries;
public class GetAllAccountQueriesCommand : IRequest<List<Account>>
{
    public Guid Id { get; set; }
}
