using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Leases.Create;

internal sealed class CreateLeaseCommandValidator : AbstractValidator<CreateLeaseCommand>
{
    public CreateLeaseCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
    {
        RuleFor(item => item.OwnerId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseCommand.OwnerId)).Description);

        RuleFor(item => item.ClientId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseCommand.ClientId)).Description);

        RuleFor(item => item.Items)
            .NotEmpty()
            .WithMessage(sharedViewLocalizer.IsRequired(nameof(CreateLeaseCommand.Items)).Description);

        RuleFor(item => item.AgentId)
            .Must(item => item != null ? item != Guid.Empty : true)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseCommand.AgentId)).Description);

        RuleFor(item => item.ContractNumber)
            .MaximumLength(100)
            .When(item => !string.IsNullOrEmpty(item.ContractNumber))
            .WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateLeaseCommand.ContractNumber), 100).Description);

        RuleFor(item => item.PaymentDay)
            .InclusiveBetween((short)1, (short)28)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseCommand.PaymentDay)).Description);

        RuleFor(item => item.EndDate)
            .Must((command, endDate) => endDate == null || endDate > command.StartDate)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseCommand.EndDate)).Description);

        RuleForEach(item => item.Items).SetValidator(new CreateLeaseItemDtoValidator(sharedViewLocalizer));
    }
}

internal sealed class CreateLeaseItemDtoValidator : AbstractValidator<CreateLeaseItemDto>
{
    public CreateLeaseItemDtoValidator(ISharedViewLocalizer sharedViewLocalizer)
    {
        RuleFor(item => item.ListingId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseItemDto.ListingId)).Description);

        RuleFor(item => item.RealEstateId)
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseItemDto.RealEstateId)).Description);

        RuleFor(item => item.RealEstateUnitId)
            .Must(item => item == null || item != Guid.Empty)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseItemDto.RealEstateUnitId)).Description);

        RuleFor(item => item.MonthlyRent)
            .GreaterThan(0)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseItemDto.MonthlyRent)).Description);

        RuleFor(item => item.DepositAmount)
            .Must(x => x > 0)
            .When(item => item.DepositAmount.HasValue)
            .WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLeaseItemDto.DepositAmount)).Description);
    }
}
