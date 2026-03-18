namespace Building.Domain.Listings;

public enum UtilityPaymentType
{
	ByOwner = 0,        // От владельца
	ByTenant,           // От арендатора
	Negotiable          // Договорная
}
