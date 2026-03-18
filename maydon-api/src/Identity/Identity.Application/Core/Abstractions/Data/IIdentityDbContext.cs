using Core.Application.Abstractions.Data;
using Identity.Application.Users.GetPermissions;
using Identity.Domain.Accounts;
using Identity.Domain.BankProperties;
using Identity.Domain.Companies;
using Identity.Domain.CompanyUsers;
using Identity.Domain.IntegrationService;
using Identity.Domain.Invitations;
using Identity.Domain.OtpContents;
using Identity.Domain.Otps;
using Identity.Domain.Roles;
using Identity.Domain.Sessions;
using Identity.Domain.Users;
using Identity.Domain.UserStates;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Core.Abstractions.Data;

public interface IIdentityDbContext : IApplicationDbContext
{
	DbSet<Account> Accounts { get; }
	DbSet<BankProperty> BankProperties { get; }
	DbSet<Company> Companies { get; }
	DbSet<CompanyUser> CompanyUsers { get; }
	DbSet<IntegrationService> IntegrationServices { get; }
	DbSet<Invitation> Invitations { get; }
	DbSet<Otp> Otps { get; }
	DbSet<OtpContent> OtpContents { get; }
	DbSet<Role> Roles { get; }
	DbSet<Session> Sessions { get; }
	DbSet<User> Users { get; }
	DbSet<UserState> UserStates { get; }
	IEnumerable<string> GetPermissionNamesByRoleId(Guid roleId);
	Task<List<GetPermissionsResponse>> GetPermissionsByRoleIdAsync(Guid roleId);
}
