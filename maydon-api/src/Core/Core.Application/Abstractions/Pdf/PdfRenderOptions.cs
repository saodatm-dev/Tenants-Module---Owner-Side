namespace Core.Application.Abstractions.Pdf;

/// <summary>
/// Options for PDF rendering via Gotenberg
/// </summary>
public record PdfRenderOptions
{
	public string PaperWidth { get; set; } = "8.27";
	public string PaperHeight { get; set; } = "11.69";
	public string MarginTop { get; set; } = "0";
	public string MarginBottom { get; set; } = "0";
	public string MarginLeft { get; set; } = "0";
	public string MarginRight { get; set; } = "0";
	public bool Landscape { get; set; } = false;
	public bool PrintBackground { get; set; } = true;
	public string Scale { get; set; } = "1.0";
	public string? PdfFormat { get; set; }
	public string? WaitDelay { get; set; }
	public string? WaitForExpression { get; set; }
	public string? HeaderHtml { get; set; }
	public string? FooterHtml { get; set; }
	public bool PreferCssPageSize { get; set; } = false;
	public bool GenerateDocumentOutline { get; set; } = false;
	public bool SkipNetworkIdleEvent { get; set; } = false;
	public bool OmitBackground { get; set; } = false;

	public static PdfRenderOptions A4Portrait => new()
	{
		PaperWidth = "8.27",
		PaperHeight = "11.69",
		MarginTop = "0",
		MarginBottom = "0",
		MarginLeft = "0",
		MarginRight = "0",
		Landscape = false,
		PrintBackground = true,
		PreferCssPageSize = true
	};

	public static PdfRenderOptions A4PortraitStandard => new()
	{
		PaperWidth = "8.27",
		PaperHeight = "11.69",
		MarginTop = "0.39",
		MarginBottom = "0.39",
		MarginLeft = "0.39",
		MarginRight = "0.39",
		Landscape = false,
		PrintBackground = true
	};

	public static PdfRenderOptions A4Landscape => new()
	{
		PaperWidth = "11.69",
		PaperHeight = "8.27",
		MarginTop = "0.2",
		MarginBottom = "0.2",
		MarginLeft = "0.2",
		MarginRight = "0.2",
		Landscape = true,
		PrintBackground = true
	};
}
