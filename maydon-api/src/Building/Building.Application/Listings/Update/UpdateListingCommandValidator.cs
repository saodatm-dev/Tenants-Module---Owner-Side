using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Listings.Update;

internal sealed class UpdateListingCommandValidator : AbstractValidator<UpdateListingCommand>
{
	public UpdateListingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.Id)).Description);

		RuleFor(item => item.RealEstateId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.RealEstateId)).Description);

		RuleFor(item => item.ListingCategoryIds)
			.NotEmpty()
			.Must(item => item.All(c => c != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.ListingCategoryIds)).Description);

		RuleFor(item => item.FloorIds)
			.Must(item => item.All(c => c != Guid.Empty))
			.When(item => item.FloorIds?.Any() == true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.FloorIds)).Description);

		RuleFor(item => item.RoomIds)
			.Must(item => item.All(c => c != Guid.Empty))
			.When(item => item.RoomIds?.Any() == true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.RoomIds)).Description);

		RuleFor(item => item.UnitIds)
			.Must(item => item.All(c => c != Guid.Empty))
			.When(item => item.UnitIds?.Any() == true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.UnitIds)).Description);

		RuleFor(item => item.AmenityIds)
			.Must(item => item.All(c => c != Guid.Empty))
			.When(item => item.AmenityIds?.Any() == true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.AmenityIds)).Description);

		RuleFor(item => item.PriceForMonth)
			.GreaterThan(0)
			.When(item => item is not null)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.PriceForMonth)).Description);

		RuleFor(item => item.PricePerSquareMeter)
			.GreaterThan(0)
			.When(item => item is not null)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.PricePerSquareMeter)).Description);

		RuleFor(item => item.Description)
			.MaximumLength(1000)
			.When(item => !string.IsNullOrEmpty(item.Description))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateListingCommand.Description), 1000).Description);

		RuleFor(item => item.RentalPurposeId)
			.NotEqual(Guid.Empty)
			.When(item => item.RentalPurposeId.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.RentalPurposeId)).Description);

		RuleFor(item => item.MinLeaseTerm)
			.IsInEnum()
			.When(item => item.MinLeaseTerm.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.MinLeaseTerm)).Description);

		RuleFor(item => item.UtilityPaymentType)
			.IsInEnum()
			.When(item => item.UtilityPaymentType.HasValue)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCommand.UtilityPaymentType)).Description);
	}
}
