using System.Text.RegularExpressions;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Contract.Contracts.Queries;
using Document.Contract.Contracts.Responses;
using Document.Contract.Gateways;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.Contracts.Queries.Prefill;

/// <summary>
/// Loads the template body, resolves placeholders from lease/company data,
/// and returns the pre-filled body for the UI to review and edit.
/// </summary>
public sealed class PrefillContractQueryHandler(
    IDocumentDbContext dbContext,
    IContractPlaceholderResolver placeholderResolver,
    IExecutionContextProvider executionContext,
    ISharedViewLocalizer sharedViewLocalizer)
    : IQueryHandler<PrefillContractQuery, PrefillContractResponse>
{
    public async Task<Result<PrefillContractResponse>> Handle(PrefillContractQuery query, CancellationToken cancellationToken)
    {
        // 1. Load the template
        var template = await dbContext.ContractTemplates
            .AsNoTracking()
            .Where(t => t.Id == query.TemplateId && t.IsActive)
            .Select(t => new { t.Bodies })
            .FirstOrDefaultAsync(cancellationToken);

        if (template is null)
            return Result.Failure<PrefillContractResponse>(
                sharedViewLocalizer.TemplateNotFound(nameof(query.TemplateId)));

        // 2. Resolve all placeholders from backend data
        var placeholders = await placeholderResolver.ResolveAllAsync(
            query.LeaseId,
            executionContext.TenantId,
            cancellationToken);

        // 3. Replace placeholders in the template body
        var resolvedBody = ReplacePlaceholders(template.Bodies, placeholders);

        return Result.Success(new PrefillContractResponse
        {
            ResolvedBody = resolvedBody,
            PlaceholderValues = placeholders.AsReadOnly()
        });
    }

    private static readonly Regex PlaceholderRegex = new(@"\{\{(?<name>[^}]+)\}\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static string ReplacePlaceholders(string body, Dictionary<string, object?> placeholders)
    {
        return PlaceholderRegex.Replace(body, match =>
        {
            var name = match.Groups["name"].Value;
            return placeholders.TryGetValue(name, out var value)
                ? value?.ToString() ?? ""
                : match.Value;
        });
    }
}
