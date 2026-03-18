using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Identity.Application.CompanyUsers.Remove;

public sealed record RemoveCompanyUserCommand([Required] Guid UserId) : ICommand<Guid>;
