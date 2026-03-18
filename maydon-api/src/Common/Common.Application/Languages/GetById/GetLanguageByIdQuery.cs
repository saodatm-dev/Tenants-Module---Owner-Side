using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Common.Application.Languages.GetById;

public sealed record GetLanguageByIdQuery([property: Required] Guid Id) : IQuery<GetLanguageByIdResponse>;
