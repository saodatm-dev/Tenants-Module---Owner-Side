using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Identity.Application.BankProperties.GetById;

public sealed record GetBankPropertyByIdQuery([property: Required] Guid Id) : IQuery<GetBankPropertyByIdResponse>;
