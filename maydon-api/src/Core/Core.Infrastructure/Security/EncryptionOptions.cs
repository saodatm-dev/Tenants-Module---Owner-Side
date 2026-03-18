using System.IO.Compression;

namespace Core.Infrastructure.Security;

public class EncryptionOptions
{
	public required string PassKey { get; set; }
	public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;
	public bool EnableCompression { get; set; } = true;
}
