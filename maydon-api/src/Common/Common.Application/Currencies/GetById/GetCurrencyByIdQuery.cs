using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Common.Application.Currencies.GetById;

public sealed record GetCurrencyByIdQuery([property: Required] Guid Id) : IQuery<GetCurrencyByIdResponse>;
