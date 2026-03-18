using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Companies.GetById;

public sealed record GetCompanyByIdQuery([property: Required] Guid Id) : IQuery<GetCompanyByIdResponse>;
