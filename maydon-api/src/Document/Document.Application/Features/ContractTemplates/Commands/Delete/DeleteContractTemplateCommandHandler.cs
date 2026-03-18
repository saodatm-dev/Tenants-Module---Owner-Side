using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Contract.ContractTemplates.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Document.Application.Features.ContractTemplates.Commands.Delete;

public sealed class DeleteContractTemplateCommandHandler(
    IDocumentDbContext db,
    IExecutionContextProvider executionContext,
    ILogger<DeleteContractTemplateCommandHandler> logger)
    : ICommandHandler<DeleteContractTemplateCommand>
{
    public async Task<Result> Handle(
        DeleteContractTemplateCommand command,
        CancellationToken cancellationToken)
    {
        var template = await db.ContractTemplates
            .FirstOrDefaultAsync(t => t.Id == command.Id &&
                                      t.TenantId == executionContext.TenantId,
                cancellationToken);

        if (template is null)
        {
            return Result.Failure(Error.NotFound(
                "ContractTemplate.NotFound",
                $"Contract template {command.Id} not found."));
        }

        template.IsDeleted = true;
        template.DeletedAt = DateTime.UtcNow;
        template.DeletedBy = executionContext.UserId;

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Soft-deleted contract template {TemplateId} by user {UserId}",
            template.Id, executionContext.UserId);

        return Result.Success();
    }
}
