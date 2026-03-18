using System.Diagnostics.Metrics;

namespace Core.Infrastructure.Monitoring;

/// <summary>
/// Centralized metrics for integration event monitoring.
/// Provides observability into event throughput, latency, and errors.
/// Compatible with OpenTelemetry, Prometheus, and Application Insights.
/// </summary>
public sealed class IntegrationEventMetrics
{
    private readonly Meter _meter;
    
    // Counters
    private readonly Counter<long> _eventsPublished;
    private readonly Counter<long> _eventsProcessed;
    private readonly Counter<long> _eventsFailed;
    private readonly Counter<long> _eventsRetried;
    private readonly Counter<long> _eventsSentToDlq;
    private readonly Counter<long> _duplicateEventsSkipped;
    
    // Histograms for latency tracking
    private readonly Histogram<double> _publishDuration;
    private readonly Histogram<double> _processingDuration;
    private readonly Histogram<double> _endToEndDuration;
    
    // Gauges (using UpDownCounters as approximation)
    private readonly UpDownCounter<long> _pendingMessages;
    private readonly UpDownCounter<long> _activeHandlers;

    public IntegrationEventMetrics()
    {
        _meter = new Meter("Maydon.IntegrationEvents", "1.0.0");

        // Initialize counters
        _eventsPublished = _meter.CreateCounter<long>(
            "integration_events.published",
            description: "Total number of integration events published");

        _eventsProcessed = _meter.CreateCounter<long>(
            "integration_events.processed",
            description: "Total number of integration events successfully processed");

        _eventsFailed = _meter.CreateCounter<long>(
            "integration_events.failed",
            description: "Total number of integration events that failed processing");

        _eventsRetried = _meter.CreateCounter<long>(
            "integration_events.retried",
            description: "Total number of integration event retry attempts");

        _eventsSentToDlq = _meter.CreateCounter<long>(
            "integration_events.dlq",
            description: "Total number of integration events sent to Dead Letter Queue");

        _duplicateEventsSkipped = _meter.CreateCounter<long>(
            "integration_events.duplicates_skipped",
            description: "Total number of duplicate events skipped (idempotency)");

        // Initialize histograms
        _publishDuration = _meter.CreateHistogram<double>(
            "integration_events.publish_duration",
            unit: "ms",
            description: "Time taken to publish an integration event");

        _processingDuration = _meter.CreateHistogram<double>(
            "integration_events.processing_duration",
            unit: "ms",
            description: "Time taken to process an integration event");

        _endToEndDuration = _meter.CreateHistogram<double>(
            "integration_events.end_to_end_duration",
            unit: "ms",
            description: "End-to-end latency from publish to processing completion");

        // Initialize gauges
        _pendingMessages = _meter.CreateUpDownCounter<long>(
            "integration_events.pending",
            description: "Current number of pending integration events");

        _activeHandlers = _meter.CreateUpDownCounter<long>(
            "integration_events.active_handlers",
            description: "Current number of active event handlers");
    }

    #region Publishing Metrics

    public void RecordEventPublished(string eventType, double durationMs)
    {
        _eventsPublished.Add(1, 
            new KeyValuePair<string, object?>("event_type", eventType));
        
        _publishDuration.Record(durationMs,
            new KeyValuePair<string, object?>("event_type", eventType));
    }

    public void RecordBatchPublished(string eventType, int count, double durationMs)
    {
        _eventsPublished.Add(count,
            new KeyValuePair<string, object?>("event_type", eventType),
            new KeyValuePair<string, object?>("batch", "true"));
        
        _publishDuration.Record(durationMs,
            new KeyValuePair<string, object?>("event_type", eventType),
            new KeyValuePair<string, object?>("batch", "true"));
    }

    #endregion

    #region Processing Metrics

    public void RecordEventProcessed(string eventType, string handlerType, double durationMs, double endToEndMs)
    {
        _eventsProcessed.Add(1,
            new KeyValuePair<string, object?>("event_type", eventType),
            new KeyValuePair<string, object?>("handler_type", handlerType));
        
        _processingDuration.Record(durationMs,
            new KeyValuePair<string, object?>("event_type", eventType),
            new KeyValuePair<string, object?>("handler_type", handlerType));
        
        _endToEndDuration.Record(endToEndMs,
            new KeyValuePair<string, object?>("event_type", eventType));
    }

    public void RecordEventFailed(string eventType, string handlerType, string errorType)
    {
        _eventsFailed.Add(1,
            new KeyValuePair<string, object?>("event_type", eventType),
            new KeyValuePair<string, object?>("handler_type", handlerType),
            new KeyValuePair<string, object?>("error_type", errorType));
    }

    public void RecordEventRetried(string eventType, int retryAttempt)
    {
        _eventsRetried.Add(1,
            new KeyValuePair<string, object?>("event_type", eventType),
            new KeyValuePair<string, object?>("retry_attempt", retryAttempt));
    }

    public void RecordEventSentToDlq(string eventType, string reason)
    {
        _eventsSentToDlq.Add(1,
            new KeyValuePair<string, object?>("event_type", eventType),
            new KeyValuePair<string, object?>("reason", reason));
    }

    public void RecordDuplicateEventSkipped(string eventType)
    {
        _duplicateEventsSkipped.Add(1,
            new KeyValuePair<string, object?>("event_type", eventType));
    }

    #endregion

    #region Queue Metrics

    public void IncrementPendingMessages(string eventType, int count = 1)
    {
        _pendingMessages.Add(count,
            new KeyValuePair<string, object?>("event_type", eventType));
    }

    public void DecrementPendingMessages(string eventType, int count = 1)
    {
        _pendingMessages.Add(-count,
            new KeyValuePair<string, object?>("event_type", eventType));
    }

    public void IncrementActiveHandlers()
    {
        _activeHandlers.Add(1);
    }

    public void DecrementActiveHandlers()
    {
        _activeHandlers.Add(-1);
    }

    #endregion

    public void Dispose()
    {
        _meter?.Dispose();
    }
}
