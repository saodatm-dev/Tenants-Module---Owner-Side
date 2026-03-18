namespace Building.Domain.MeterTariffs;

/// <summary>
/// Сезонность тарифа - определяет период действия тарифа
/// </summary>
public enum Season
{
	All,         // Круглогодичный тариф
	Summer,      // Март–Октябрь (летний период в Узбекистане)
	Winter       // Ноябрь–Февраль (отопительный сезон)
}
