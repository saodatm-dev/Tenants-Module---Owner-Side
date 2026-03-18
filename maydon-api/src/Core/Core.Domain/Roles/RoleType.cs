namespace Core.Domain.Roles;

[Flags]
public enum RoleType
{
	System = 0,
	Client,
	Owner
}
