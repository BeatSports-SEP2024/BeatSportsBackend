using System.ComponentModel.DataAnnotations;
using AutoFilterer.Types;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Authentication.Queries;
public class GetAccountByIdCommand : IRequest<AccountResponse>
{
    [Required]
    public Guid AccountId { get; set; }
}