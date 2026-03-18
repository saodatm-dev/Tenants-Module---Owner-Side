using Document.Contract.ContractTemplates.Responses;

namespace Document.Application.Features.ContractTemplates.Rendering;

/// <summary>
/// Registry of all known placeholder keys, grouped by category.
/// Used by the frontend sidebar ("Insert Field") and by the backend for validation.
/// </summary>
public static class PlaceholderRegistry
{
    private static readonly List<PlaceholderGroup> _groups =
    [
        new PlaceholderGroup
        {
            Category = "owner",
            Label = "Арендодатель / Ijara beruvchi",
            Items =
            [
                new PlaceholderItem { Key = "owner_company_name", Label = "Наименование компании", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "owner_inn", Label = "ИНН", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "owner_mfo", Label = "МФО", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "owner_bank_account", Label = "Расчётный счёт", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "owner_bank_name", Label = "Банк", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "owner_director", Label = "Директор", Type = "string", AutoResolvable = true },
            ]
        },
        new PlaceholderGroup
        {
            Category = "client",
            Label = "Арендатор / Ijara oluvchi",
            Items =
            [
                new PlaceholderItem { Key = "client_company_name", Label = "Наименование компании", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "client_inn", Label = "ИНН", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "client_mfo", Label = "МФО", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "client_bank_account", Label = "Расчётный счёт", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "client_bank_name", Label = "Банк", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "client_director", Label = "Директор", Type = "string", AutoResolvable = true },
            ]
        },
        new PlaceholderGroup
        {
            Category = "property",
            Label = "Объект / Mulk",
            Items =
            [
                new PlaceholderItem { Key = "property_address", Label = "Адрес", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "property_cadastral", Label = "Кадастровый номер", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "property_area_sqm", Label = "Площадь (м²)", Type = "decimal", AutoResolvable = true },
            ]
        },
        new PlaceholderGroup
        {
            Category = "lease",
            Label = "Договор аренды / Ijara shartnomasi",
            Items =
            [
                new PlaceholderItem { Key = "lease_start_date", Label = "Дата начала", Type = "date", AutoResolvable = true },
                new PlaceholderItem { Key = "lease_end_date", Label = "Дата окончания", Type = "date", AutoResolvable = true },
                new PlaceholderItem { Key = "lease_duration_months", Label = "Срок (мес.)", Type = "int", AutoResolvable = true },
                new PlaceholderItem { Key = "monthly_price", Label = "Ежемесячная оплата", Type = "decimal", AutoResolvable = true },
                new PlaceholderItem { Key = "total_amount", Label = "Общая сумма", Type = "decimal", AutoResolvable = true },
                new PlaceholderItem { Key = "payment_day", Label = "День оплаты", Type = "int", AutoResolvable = true },
            ]
        },
        new PlaceholderGroup
        {
            Category = "contract",
            Label = "Контракт / Shartnoma",
            Items =
            [
                new PlaceholderItem { Key = "contract_number", Label = "Номер договора", Type = "string", AutoResolvable = true },
                new PlaceholderItem { Key = "contract_date", Label = "Дата договора", Type = "date", AutoResolvable = true },
            ]
        },
        new PlaceholderGroup
        {
            Category = "custom",
            Label = "Дополнительно / Qo'shimcha",
            Items =
            [
                new PlaceholderItem { Key = "special_conditions", Label = "Особые условия", Type = "string", AutoResolvable = false },
                new PlaceholderItem { Key = "rooms", Label = "Помещения (список)", Type = "array", AutoResolvable = false },
            ]
        }
    ];

    /// <summary>
    /// Returns the full placeholder catalog.
    /// </summary>
    public static PlaceholderCatalogResponse GetCatalog() =>
        new() { Groups = _groups.AsReadOnly() };

    /// <summary>
    /// Returns all known placeholder keys.
    /// </summary>
    public static HashSet<string> GetAllKeys() =>
        _groups.SelectMany(g => g.Items.Select(i => i.Key)).ToHashSet();
}
