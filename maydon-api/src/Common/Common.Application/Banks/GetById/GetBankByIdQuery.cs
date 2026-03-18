using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Common.Application.Banks.GetById;

public sealed record GetBankByIdQuery([property: Required] Guid Id) : IQuery<GetBankByIdResponse>;
