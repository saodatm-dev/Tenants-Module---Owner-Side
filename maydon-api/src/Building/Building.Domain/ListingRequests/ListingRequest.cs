using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Listings;
using Core.Domain.Entities;

namespace Building.Domain.ListingRequests;

[Table("listing_requests", Schema = AssemblyReference.Instance)]
public sealed class ListingRequest : Entity
{
	private ListingRequest() { }
	public ListingRequest(
		Guid listingId,
		Guid ownerId,
		Guid clientId,
		string content) : base()
	{
		ListingId = listingId;
		OwnerId = ownerId;
		ClientId = clientId;
		Content = content;
		Status = ListingRequestStatus.Sent;
		Reason = string.Empty;
	}
	public Guid ListingId { get; private set; }
	public Guid OwnerId { get; private set; }
	public Guid ClientId { get; private set; }
	[Required]
	[MaxLength(2000)]
	public string Content { get; private set; }
	public ListingRequestStatus Status { get; private set; }
	[MaxLength(500)]
	public string Reason { get; private set; }
	public Listing Listing { get; private set; }

	public ListingRequest Update(string content)
	{
		Content = content;
		Status = ListingRequestStatus.Sent;
		return this;
	}
	public bool IsSent() => Status == ListingRequestStatus.Sent;
	public bool IsReceive() => Status == ListingRequestStatus.Received;
	public bool IsAccept() => Status == ListingRequestStatus.Accepted;
	public bool IsCancel() => Status == ListingRequestStatus.Canceled;
	public bool IsReject() => Status == ListingRequestStatus.Rejected;

	public ListingRequest Send()
	{
		Status = ListingRequestStatus.Sent;
		Reason = string.Empty;
		return this;
	}
	public ListingRequest Received()
	{
		Status = ListingRequestStatus.Received;
		return this;
	}
	public ListingRequest Accept()
	{
		Status = ListingRequestStatus.Accepted;
		return this;
	}
	public ListingRequest Cancel()
	{
		Status = ListingRequestStatus.Canceled;
		return this;
	}
	public ListingRequest Reject(string reason)
	{
		Status = ListingRequestStatus.Rejected;
		Reason = reason;
		return this;
	}
}
