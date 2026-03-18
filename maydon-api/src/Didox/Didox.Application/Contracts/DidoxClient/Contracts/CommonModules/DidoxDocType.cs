using System.Runtime.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;

/// <summary>
/// Types of electronic documents in the Didox system. Used to identify document types when calling API and processing document responses.
/// </summary>
public enum DidoxDocType
{
    /// <summary>
    /// Invoice-factura (not used).
    /// </summary>
    [EnumMember(Value = "001")]
    InvoiceUnused,

    /// <summary>
    /// Invoice-factura without act.
    /// </summary>
    [EnumMember(Value = "002")]
    InvoiceWithoutAct,

    /// <summary>
    /// Invoice-factura (pharmaceutical).
    /// </summary>
    [EnumMember(Value = "008")]
    PharmaceuticalInvoice,

    /// <summary>
    /// Hybrid invoice-factura.
    /// </summary>
    [EnumMember(Value = "023")]
    HybridInvoice,

    /// <summary>
    /// Waybill-transport document (TTN).
    /// </summary>
    [EnumMember(Value = "041")]
    Waybill,

    /// <summary>
    /// Act of completed works.
    /// </summary>
    [EnumMember(Value = "005")]
    ActOfCompletedWorks,

    /// <summary>
    /// Power of attorney.
    /// </summary>
    [EnumMember(Value = "006")]
    Empowerment,

    /// <summary>
    /// Contract (GNK).
    /// </summary>
    [EnumMember(Value = "007")]
    ContractGnk,

    /// <summary>
    /// Custom document.
    /// </summary>
    [EnumMember(Value = "000")]
    CustomDocument,

    /// <summary>
    /// Reconciliation act.
    /// </summary>
    [EnumMember(Value = "052")]
    ReconciliationAct,

    /// <summary>
    /// Acceptance-transfer act.
    /// </summary>
    [EnumMember(Value = "054")]
    AcceptanceTransferAct,

    /// <summary>
    /// Multi-party custom document.
    /// </summary>
    [EnumMember(Value = "010")]
    MultiPartyCustomDocument,

    /// <summary>
    /// Founders meeting protocol.
    /// </summary>
    [EnumMember(Value = "075")]
    FoundersMeetingProtocol,

    /// <summary>
    /// Tax committee letter (TC).
    /// </summary>
    [EnumMember(Value = "013")]
    TaxCommitteeLetter
}
