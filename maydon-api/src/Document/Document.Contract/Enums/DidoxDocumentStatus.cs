using System.Text.Json.Serialization;

namespace Document.Contract.Enums;

/// <summary>
/// Represents the status of a Didox document (Contract copy for external services)
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DidoxDocumentStatus
{
    Draft = 0,
    AwaitingPartnerSignature = 1,
    AwaitingYourSignature = 2,
    Signed = 3,
    SignatureDeclined = 4,
    Deleted = 5,
    AwaitingAgentSignatureForSender = 6,
    Invalid = 40,
    CanceledByGnk = 50,
    AwaitingAgentSignatureForRecipient = 60,
    Dispatched = 110,
    AcceptedByResponsiblePerson = 140,
    GoodsReturnedByResponsiblePerson = 150,
    DeliveredToRecipient = 160,
    RejectedByRecipient = 170,
    GoodsReturnedByResponsiblePersonTtn = 190,
    ReturnedTtn = 200
}

