using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Listings.Create;

internal sealed class CreateListingCommandValidator : AbstractValidator<CreateListingCommand>
{
	public CreateListingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.RealEstateId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.RealEstateId)).Description);

		RuleFor(item => item.ListingCategoryIds)
			.NotEmpty()
			.Must(item => item.All(c => c != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.ListingCategoryIds)).Description);

		RuleFor(item => item.FloorIds)
			.Must(item => item.All(c => c != Guid.Empty))
			.When(item => item.FloorIds?.Any() == true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.FloorIds)).Description);

		RuleFor(item => item.RoomIds)
			.Must(item => item.All(c => c != Guid.Empty))
			.When(item => item.RoomIds?.Any() == true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.RoomIds)).Description);

		RuleFor(item => item.UnitIds)
			.Must(item => item.All(c => c != Guid.Empty))
			.When(item => item.UnitIds?.Any() == true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.UnitIds)).Description);

		RuleFor(item => item.AmenityIds)
			.Must(item => item.All(c => c != Guid.Empty))
			.When(item => item.AmenityIds?.Any() == true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.AmenityIds)).Description);

		RuleFor(item => item.PriceForMonth)
			.GreaterThan(0)
			.When(item => item is not null)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.PriceForMonth)).Description);

		RuleFor(item => item.PricePerSquareMeter)
			.GreaterThan(0)
			.When(item => item is not null)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.PricePerSquareMeter)).Description);

		RuleFor(item => item.Description)
			.MaximumLength(1000)
			.When(item => !string.IsNullOrEmpty(item.Description))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateListingCommand.Description), 1000).Description);

		RuleFor(item => item.RentalPurposeId)
			.NotEqual(Guid.Empty)
			.When(item => item.RentalPurposeId.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.RentalPurposeId)).Description);

		RuleFor(item => item.MinLeaseTerm)
			.IsInEnum()
			.When(item => item.MinLeaseTerm.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.MinLeaseTerm)).Description);

		RuleFor(item => item.UtilityPaymentType)
			.IsInEnum()
			.When(item => item.UtilityPaymentType.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCommand.UtilityPaymentType)).Description);
	}
}
