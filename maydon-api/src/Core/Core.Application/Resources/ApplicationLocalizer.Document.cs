using Core.Domain.Results;

namespace Core.Application.Resources;

/// <summary>
/// Extension methods for Document, Contract, Didox, and Audit localized error messages.
/// </summary>
public static partial class ApplicationLocalizer
{
    extension(ISharedViewLocalizer localizer)
    {
        #region Document Errors

        public Error DocumentNotFound(string parameter) =>
            Error.NotFound(parameter, localizer["Document_NotFound"].Value);

        public Error TemplateNotFound(string parameter) =>
            Error.NotFound(parameter, localizer["Template_NotFound"].Value);

        public Error ParticipantNotFound(string parameter) =>
            Error.NotFound(parameter, localizer["Participant_NotFound"].Value);

        public Error OwnerNotFound(string parameter) =>
            Error.NotFound(parameter, localizer["Owner_NotFound"].Value);

        public Error TemplateInUse(string parameter) =>
            Error.Conflict(parameter, localizer["Template_InUse"].Value);

        public Error ParticipantAlreadyExists(string parameter) =>
            Error.Conflict(parameter, localizer["Participant_AlreadyExists"].Value);

        public Error DocumentInvalidStatus(string requiredStatus, string currentStatus) =>
            Error.Validation("DocumentStatus", string.Format(localizer["Document_InvalidStatus"].Value, requiredStatus, currentStatus));

        public Error DocumentNotExported(string providerName) =>
            Error.Validation("DocumentExport", string.Format(localizer["Document_NotExported"].Value, providerName));

        public Error DocumentExportNotCompleted(string providerName, string status) =>
            Error.Validation("DocumentExport", string.Format(localizer["Document_ExportNotCompleted"].Value, providerName, status));

        #endregion

        #region Contract Errors

        public Error ContractNotFound(string parameter) =>
            Error.NotFound(parameter, localizer["Contract_Notfound"].Value);

        public Error ContractInvalidParty(string parameter) =>
            Error.Validation(parameter, localizer["Contract_InvalidParty"].Value);

        public Error ContractNoDidoxState(string parameter) =>
            Error.Validation(parameter, localizer["Contract_NoDidoxState"].Value);

        public Error ContractPdfRenderFailed(string parameter) =>
            Error.Failure(parameter, localizer["Contract_PdfRenderFailed"].Value);

        #endregion

        #region Contract Template Errors

        public Error ContractTemplateInvalidBlocks(string parameter, string details) =>
            Error.Validation(parameter, string.Format(localizer["ContractTemplate_InvalidBlocks"].Value, details));

        public Error ContractTemplateInvalidTheme(string parameter, string details) =>
            Error.Validation(parameter, string.Format(localizer["ContractTemplate_InvalidTheme"].Value, details));

        public Error ContractTemplateLanguageNotFound(string parameter, string language) =>
            Error.NotFound(parameter, string.Format(localizer["ContractTemplate_LanguageNotFound"].Value, language));

        public Error ContractTemplateInvalidManualValues(string parameter) =>
            Error.Validation(parameter, localizer["ContractTemplate_InvalidManualValues"].Value);

        public Error ContractTemplatePdfRenderFailed(string parameter) =>
            Error.Failure(parameter, localizer["ContractTemplate_PdfRenderFailed"].Value);

        #endregion

        #region Attachment Errors

        public Error AttachmentTooLarge(string parameter, long maxMb) =>
            Error.Validation(parameter, string.Format(localizer["Attachment_TooLarge"].Value, maxMb));

        public Error AttachmentInvalidType(string parameter) =>
            Error.Validation(parameter, localizer["Attachment_InvalidType"].Value);

        public Error AttachmentLimitExceeded(string parameter, int maxCount) =>
            Error.Validation(parameter, string.Format(localizer["Attachment_LimitExceeded"].Value, maxCount));

        public Error AttachmentUploadFailed(string parameter) =>
            Error.Failure(parameter, localizer["Attachment_UploadFailed"].Value);

        #endregion

        #region Lease Errors

        public Error LeaseNotFound(string parameter) =>
            Error.NotFound(parameter, localizer["Lease_NotFound"].Value);

        #endregion

        #region Didox / Integration Errors

        public Error DidoxRequestFailed(string parameter) =>
            Error.Failure(parameter, localizer["Didox_Failed"].Value);

        public Error UnAuthorizedAccess(string parameter) =>
            Error.Unauthorized(parameter, localizer["Identity_Unauthorized"].Value);

        public Error InternalServerError(string parameter) =>
            Error.Validation(parameter, localizer["Server_Failure"].Value);

        public Error MaximumLength(string parameter, int maxLength) =>
            Error.Validation(parameter, string.Format(localizer["MaximumLength"].Value, maxLength));

        public Error GreaterThanOrEqualTo(string parameter, int minValue) =>
            Error.Validation(parameter, string.Format(localizer["GreaterThanOrEqualTo"].Value, minValue));

        public Error LessThanOrEqualTo(string parameter, int maxValue) =>
            Error.Validation(parameter, string.Format(localizer["LessThanOrEqualTo"].Value, maxValue));

        public Error ResourceNotFound(string resourceName, string parameter = "")
        {
            var message = localizer["Resource_NotFound"];
            var formattedMessage = message.ResourceNotFound
                ? string.Format("{0} not found", resourceName)
                : string.Format(message.Value, resourceName);
            return Error.NotFound(parameter, formattedMessage);
        }

        public Error ResourceExists(string resourceName, string parameter)
        {
            var message = localizer["Resource_Exists"];
            var formattedMessage = message.ResourceNotFound
                ? string.Format("{0} already exists", resourceName)
                : string.Format(message.Value, resourceName);
            return Error.Conflict(parameter, formattedMessage);
        }

        #endregion

        #region Audit Log Errors

        public Error AuditSummaryEmpty(string parameter) =>
            Error.Failure(parameter, localizer["Audit_SummaryEmpty"].Value);

        public Error AuditSummaryDeserializationFailed(string parameter) =>
            Error.Failure(parameter, localizer["Audit_SummaryDeserializationFailed"].Value);

        #endregion
    }
}
