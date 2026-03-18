using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using Identity.Domain.Companies;
using Identity.Domain.Invitations.Events;
using Identity.Domain.Roles;
using Identity.Domain.Users;

namespace Identity.Domain.Invitations;

[Table("invitations", Schema = AssemblyReference.Instance)]
public sealed class Invitation : Entity
{
	private Invitation() { }
	public Invitation(
		Guid senderId,
		Guid recipientId,
		Guid roleId,
		string content,
		DateTime expiredTime) : base()
	{
		SenderId = senderId;
		RecipientId = recipientId;
		RoleId = roleId;
		Content = content;
		ExpiredTime = expiredTime;
		Status = InvitationStatus.Sent;
		Raise(new CreateInvitationDomainEvent(this));
	}
	public Invitation(
		Guid senderId,
		string recipientPhoneNumber,
		Guid roleId,
		string content,
		DateTime expiredTime) : base()
	{
		SenderId = senderId;
		RoleId = roleId;
		ReceipientPhoneNumber = recipientPhoneNumber;
		Content = content;
		ExpiredTime = expiredTime;
		Status = InvitationStatus.Sent;
		Raise(new CreateInvitationDomainEvent(this));
	}

	public Guid SenderId { get; private set; }
	public Guid RoleId { get; private set; }
	public Guid? RecipientId { get; private set; }
	[MaxLength(20)]
	public string? ReceipientPhoneNumber { get; private set; }
	[Required]
	[MaxLength(500)]
	public string Content { get; private set; }
	[MaxLength(100)]
	public string Key { get; set; }
	public DateTime ExpiredTime { get; private set; }
	public InvitationStatus Status { get; private set; }
	[MaxLength(500)]
	public string? Reason { get; private set; }                      // reject reason
	public Company Sender { get; private set; }
	public User Recipient { get; private set; }
	public Role Role { get; private set; }
	public Invitation Resend()
	{
		return this;
	}
	public Invitation Received()
	{
		Status = InvitationStatus.Received;
		return this;
	}
	public Invitation Accept()
	{
		if (this.RecipientId != null)
		{
			Status = InvitationStatus.Accepted;
			Raise(new AcceptInvitationDomainEvent(this.SenderId, this.RecipientId.Value, this.RoleId));
		}
		return this;
	}
	public Invitation Cancel()
	{
		Status = InvitationStatus.Canceled;
		return this;
	}
	public Invitation Reject(string? reason)
	{
		Status = InvitationStatus.Rejected;
		Reason = reason?.Trim();
		return this;
	}
	public Invitation LinkToUser(Guid userId)
	{
		RecipientId = userId;
		return this;
	}
	public bool IsExpired(DateTime dateTime) => this.ExpiredTime < dateTime;
}
