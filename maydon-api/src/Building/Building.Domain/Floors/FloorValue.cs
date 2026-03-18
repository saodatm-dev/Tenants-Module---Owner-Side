namespace Building.Domain.Floors;

public sealed record FloorValue(
	Guid? FloorId,
	short Number,
	float? LivingArea,
	float? TotalArea,
	float? CeilingHeight,
	string? Plan);
