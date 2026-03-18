using Building.Domain.Rooms;
using Core.Application.Resources;
using FluentValidation;
using FluentValidation.Results;

namespace Building.Application.Core.Validators;

internal static class BuildingValidator
{
	private const short MinimumFloorsNumber = -3;
	private const short MaximumFloorsNumber = 100;
	private const short MinimumRoomsCount = 1;      // bathroom,kitchen,bedroom
	private const short MaximumRoomsCount = 50;     // Castle :-)
	private const short MinimumArea = 0;
	private const short MaximumArea = 10000;        // meter square

	private const short MinimumLatitudeValue = -90;
	private const short MaximumLatitudeValue = 90;

	private const short MinimumLongitudeValue = -180;
	private const short MaximumLongitudeValue = 180;

	extension<T>(IRuleBuilder<T, double> ruleBuilder)
	{
		public IRuleBuilderOptionsConditions<T, double> Latitude(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom<T, double>((value, context) =>
			{
				if (MaximumLatitudeValue < value || value < MinimumLatitudeValue)
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}

		public IRuleBuilderOptionsConditions<T, double> Longitude(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom<T, double>((value, context) =>
			{
				if (MaximumLongitudeValue < value || value < MinimumLongitudeValue)
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}
	}

	extension<T>(IRuleBuilder<T, short?> ruleBuilder)
	{
		public IRuleBuilderOptionsConditions<T, short?> FloorNumber(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom<T, short?>((value, context) =>
			{
				if (value is not null && (MaximumFloorsNumber < value || value < MinimumFloorsNumber))
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}

		public IRuleBuilderOptionsConditions<T, short?> RoomsCount(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom<T, short?>((value, context) =>
			{
				if (value is not null && (MaximumRoomsCount < value || value < MinimumRoomsCount))
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}
	}

	extension<T>(IRuleBuilder<T, double?> ruleBuilder)
	{
		public IRuleBuilderOptionsConditions<T, double?> NullableLatitude(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom<T, double?>((value, context) =>
			{
				if (value is not null && (MaximumLatitudeValue < value || value < MinimumLatitudeValue))
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}

		public IRuleBuilderOptionsConditions<T, double?> NullableLongitude(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom<T, double?>((value, context) =>
			{
				if (value is not null && (MaximumLongitudeValue < value || value < MinimumLongitudeValue))
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}
	}
	extension<T>(IRuleBuilder<T, float> ruleBuilder)
	{
		public IRuleBuilderOptionsConditions<T, float> TotalArea(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom<T, float>((value, context) =>
			{
				if (MaximumArea < value || value < MinimumArea)
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}
	}
	extension<T>(IRuleBuilder<T, float?> ruleBuilder)
	{
		public IRuleBuilderOptionsConditions<T, float?> LivingArea(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom<T, float?>((value, context) =>
			{
				if (MaximumArea < value || value < MinimumArea)
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}
	}

	extension<T>(IRuleBuilder<T, IEnumerable<RoomValue>?> ruleBuilder)
	{
		public IRuleBuilderOptionsConditions<T, IEnumerable<RoomValue>?> Rooms(ISharedViewLocalizer sharedViewLocalizer)
		{
			return ruleBuilder.Custom<T, IEnumerable<RoomValue>?>((value, context) =>
			{
				if (value is not null && (MaximumRoomsCount < value.Count() || value.Count() < MaximumRoomsCount))
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
				if (value is not null && !value.All(item => MinimumArea < item.Area && item.Area < MaximumArea && item.RoomTypeId != Guid.Empty))
				{
					var error = sharedViewLocalizer.InvalidValue(context.PropertyPath);
					var validationFailure = new ValidationFailure(error.Code, error.Description)
					{
						ErrorCode = error.Code
					};
					context.AddFailure(validationFailure);
				}
			});
		}
	}
}
