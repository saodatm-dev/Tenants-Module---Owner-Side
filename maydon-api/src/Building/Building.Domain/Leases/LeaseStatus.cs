namespace Building.Domain.Leases;

public enum LeaseStatus
{
	Pending = 1,            // Ожидает — awaiting contract creation
	ContractPending,        // Контракт в процессе — contract being signed in Didox
	Active,                 // Активный — contract executed, lease running
	Suspended,              // Приостановлен — temporarily paused
	Revoked,                // Отозван — early termination
	Expired,                // Истёк — lease end date reached
	Archived,               // Архивирован — moved to cold storage
}
