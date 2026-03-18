using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstates.Reject;

public sealed record RejectModerationRealEstateCommand(Guid Id, string? Reason) : ICommand<Guid>;
