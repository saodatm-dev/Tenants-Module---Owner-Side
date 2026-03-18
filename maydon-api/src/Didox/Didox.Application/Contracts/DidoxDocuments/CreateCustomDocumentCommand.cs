namespace Didox.Application.Contracts.DidoxDocuments;

public record CreateCustomDocumentCommand(Guid MaydonDocId, Guid DocumentTemplateId);
