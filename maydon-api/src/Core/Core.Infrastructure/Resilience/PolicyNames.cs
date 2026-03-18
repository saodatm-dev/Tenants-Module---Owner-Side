namespace Core.Infrastructure.Resilience;

/// <summary>
/// Well-known policy names for the application
/// </summary>
public static class PolicyNames
{
    public const string ExternalApiCall = "ExternalApiCall";
    public const string DatabaseOperation = "DatabaseOperation";
    public const string FileStorage = "FileStorage";
    public const string MessagePublishing = "MessagePublishing";
    public const string DidoxApiCall = "DidoxApiCall";
}
