using Core.Application.Abstractions.Jobs;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Notifications;
using Core.Application.Abstractions.Repositories;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Jobs;

/// <summary>
/// Orchestrates the versioning workflow: persistence to cold/hot storage and notifications.
/// </summary>
public sealed class VersioningWorkflowProcessor(
    IVersionRepository coldStorageRepository,
    IEntityChangeNotificationHandler notificationHandler,
    ILogger<VersioningWorkflowProcessor> logger) : IVersioningWorkflowProcessor
{
    public async Task ExecuteAsync(EntityVersionChangedEvent @event, CancellationToken cancellationToken)
    {
        var tasks = new[]
        {
            new WorkflowTask("MongoDB", true, () => coldStorageRepository.SaveVersionAsync(@event, cancellationToken)),
            new WorkflowTask("Notification", false, () => notificationHandler.HandleAsync(@event, cancellationToken))
        };

        var results = await Task.WhenAll(tasks.Select(ExecuteTaskAsync));

        var criticalFailures = results
            .Where(r => r is { IsCritical: true, Success: false })
            .ToList();

        if (criticalFailures.Any())
        {
            var failedNames = string.Join(", ", criticalFailures.Select(f => f.Name));
            throw new InvalidOperationException($"Critical operations failed: {failedNames}");
        }

        LogWorkflowResults(@event, results);
    }

    private async Task<WorkflowResult> ExecuteTaskAsync(WorkflowTask task)
    {
        try
        {
            await task.Execute();
            return new WorkflowResult(task.Name, true, task.IsCritical, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{TaskName} failed", task.Name);
            return new WorkflowResult(task.Name, false, task.IsCritical, ex);
        }
    }

    private void LogWorkflowResults(EntityVersionChangedEvent @event, WorkflowResult[] results)
    {
        var resultsSummary = string.Join(", ", results.Select(r => $"{r.Name}:{r.Success} Error: {r.Error?.Message}"));

        if (results.All(r => r.Success))
        {
            logger.LogInformation(
                "Versioning workflow completed successfully for {EntityType} {EntityId} v{Version} ({Results})",
                @event.EntityType, @event.EntityId, @event.VersionNumber, resultsSummary);
        }
        else
        {
            logger.LogWarning(
                "Versioning workflow completed with partial failures for {EntityType} {EntityId} v{Version} ({Results})",
                @event.EntityType, @event.EntityId, @event.VersionNumber, resultsSummary);
        }
    }

    private record WorkflowTask(string Name, bool IsCritical, Func<Task> Execute);

    private record WorkflowResult(string Name, bool Success, bool IsCritical, Exception? Error);
}
