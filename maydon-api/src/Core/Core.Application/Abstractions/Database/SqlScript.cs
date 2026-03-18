namespace Core.Application.Abstractions.Database;

public class SqlScript
{
	public string FileName { get; set; } = string.Empty;
	public string FilePath { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public string ResourceName { get; set; } = string.Empty;
}
