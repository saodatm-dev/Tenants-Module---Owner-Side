using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Complexes.GetById;

public sealed record GetComplexByIdQuery([property: Required] Guid Id) : IQuery<GetComplexByIdResponse>;
