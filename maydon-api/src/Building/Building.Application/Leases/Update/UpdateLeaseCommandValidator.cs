using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Leases.Update;

internal sealed class UpdateLeaseCommandValidator : AbstractValidator<UpdateLeaseCommand>
{
    public UpdateLeaseCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
    {
        RuleFor(item => item.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseCommand.Id)).Description);

        RuleFor(item => item.OwnerId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseCommand.OwnerId)).Description);

        RuleFor(item => item.ClientId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseCommand.ClientId)).Description);

        RuleFor(item => item.Items)
            .NotEmpty()
            .WithMessage(sharedViewLocalizer.IsRequired(nameof(UpdateLeaseCommand.Items)).Description);

        RuleFor(item => item.AgentId)
            .Must(item => item != null ? item != Guid.Empty : true)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseCommand.AgentId)).Description);

        RuleFor(item => item.ContractNumber)
            .MaximumLength(100)
            .When(item => !string.IsNullOrEmpty(item.ContractNumber))
            .WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateLeaseCommand.ContractNumber), 100).Description);

        RuleFor(item => item.PaymentDay)
            .InclusiveBetween((short)1, (short)28)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseCommand.PaymentDay)).Description);

        RuleFor(item => item.EndDate)
            .Must((command, endDate) => endDate == null || endDate > command.StartDate)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseCommand.EndDate)).Description);

        RuleForEach(item => item.Items).SetValidator(new UpdateLeaseItemDtoValidator(sharedViewLocalizer));
    }
}

internal sealed class UpdateLeaseItemDtoValidator : AbstractValidator<UpdateLeaseItemDto>
{
    public UpdateLeaseItemDtoValidator(ISharedViewLocalizer sharedViewLocalizer)
    {
        RuleFor(item => item.ListingId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseItemDto.ListingId)).Description);

        RuleFor(item => item.RealEstateId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseItemDto.RealEstateId)).Description);

        RuleFor(item => item.RealEstateUnitId)
            .Must(item => item != null ? item != Guid.Empty : true)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseItemDto.RealEstateUnitId)).Description);

        RuleFor(item => item.MonthlyRent)
            .Must(x => x.Amount > 0)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseItemDto.MonthlyRent)).Description);

        RuleFor(item => item.DepositAmount)
            .Must(x => x.Amount >= 0)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLeaseItemDto.DepositAmount)).Description);
    }
}
