namespace Core.Domain.Security;

public sealed record EncryptionResult(
	byte[] Data,
	int OriginalSize,
	int CompressedSize,
	int EncryptedSize)
{
	public double CompressionRatio =>
		OriginalSize > 0 ? 1 - (double)CompressedSize / OriginalSize : 0;

	public string Summary =>
		$"Original: {OriginalSize:N0}B → Compressed: {CompressedSize:N0}B → Encrypted: {EncryptedSize:N0}B ({CompressionRatio:P0} saved)";
}
