namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Request.Products;

public class ProductItem
{
    public int OrdNo { get; set; }
    public string? LgotaId { get; set; }

    public string? CommittentName { get; set; }
    public string? CommittentTin { get; set; }
    public string? CommittentVatRegCode { get; set; }
    public string? CommittentVatRegStatus { get; set; }

    public string? Name { get; set; }
    public string? CatalogCode { get; set; }
    public string? CatalogName { get; set; }

    public string? Marks { get; set; }
    public string? Barcode { get; set; }

    public string? MeasureId { get; set; }

    public string? PackageCode { get; set; }
    public string? PackageName { get; set; }

    public string? Count { get; set; }
    public string? Summa { get; set; }
    public string? DeliverySum { get; set; }

    public string? VatRate { get; set; }
    public string? VatSum { get; set; }

    public decimal ExciseRate { get; set; }
    public decimal ExciseSum { get; set; }

    public string? DeliverySumWithVat { get; set; }

    public bool WithoutVat { get; set; }
    public bool WithoutExcise { get; set; }

    public int? LgotaType { get; set; }
    public string? LgotaName { get; set; }
    public decimal LgotaVatSum { get; set; }

    public string? WarehouseId { get; set; }
    public int Origin { get; set; }
}
