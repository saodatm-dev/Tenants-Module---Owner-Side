using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Companies.Remove;

public sealed record RemoveCompanyCommand([Required] Guid Id) : ICommand<Guid>;
