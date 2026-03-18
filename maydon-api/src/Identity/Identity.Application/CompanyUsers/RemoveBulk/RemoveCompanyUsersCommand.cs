using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Identity.Application.CompanyUsers.RemoveBulk;

public sealed record RemoveCompanyUsersCommand([Required] IEnumerable<Guid> UserIds) : ICommand;
