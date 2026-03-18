using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstateUnits.Reject;

public sealed record RejectModerationRealEstateUnitCommand(Guid Id, string? Reason) : ICommand<Guid>;
