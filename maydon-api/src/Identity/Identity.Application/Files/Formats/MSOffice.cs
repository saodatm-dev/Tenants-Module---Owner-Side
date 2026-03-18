namespace Identity.Application.Files.Formats;

internal sealed class MSOffice : FileFormatDescriptor
{
	protected override void Initialize()
	{
		TypeName = "MS office file";
		Extensions.UnionWith(new[] { ".xls", ".xlsx" });
		MagicNumbers.AddRange(new byte[][]
		{
			new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 },
			new byte[] { 0x50, 0x4B, 0x03, 0x04 }
		});
	}
}
