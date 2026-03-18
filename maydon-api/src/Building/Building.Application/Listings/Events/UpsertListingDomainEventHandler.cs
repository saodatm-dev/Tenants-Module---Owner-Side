using Building.Application.Core.Abstractions.Data;
using Building.Domain.ListingAmenities;
using Building.Domain.Listings;
using Building.Domain.Listings.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Listings.Events;

internal sealed class UpsertListingDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<UpsertListingDomainEvent>
{
	public async ValueTask Handle(UpsertListingDomainEvent @event, CancellationToken cancellationToken)
	{
		await UpsertListingAmenitiesAsync(@event, cancellationToken);
		await UpsertListingDescriptionTranslatesAsync(@event, cancellationToken);

		await ValueTask.CompletedTask;
	}
	private async Task UpsertListingAmenitiesAsync(UpsertListingDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.AmenityIds?.Any() == true)
		{
			var existListingAmenities = await dbContext.ListingAmenities
				.Where(item => item.ListingId == @event.Listing.Id)
				.ToListAsync(cancellationToken);

			if (existListingAmenities.Count > 0)
			{
				var newListingAmenityIds = @event.AmenityIds.Where(amenityId => !existListingAmenities.Any(r => r.AmenityId == amenityId));
				var deleteListingAmenities = existListingAmenities.Where(item => !@event.AmenityIds.Any(amenityId => amenityId == item.AmenityId));

				if (newListingAmenityIds?.Any() == true)
				{
					await dbContext.ListingAmenities.AddRangeAsync(
						newListingAmenityIds.Select(amenityId =>
						new ListingAmenity(
							@event.Listing.Id,
							amenityId
							)), cancellationToken);
				}
				if (deleteListingAmenities?.Any() == true)
					dbContext.ListingAmenities.RemoveRange(deleteListingAmenities);
			}
			else
			{
				await dbContext.ListingAmenities.AddRangeAsync(
						@event.AmenityIds.Select(amenityId =>
						new ListingAmenity(
							@event.Listing.Id,
							amenityId
							)), cancellationToken);
			}
		}

		await ValueTask.CompletedTask;
	}

	private async Task UpsertListingDescriptionTranslatesAsync(UpsertListingDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.DescriptionTranslates?.Any() != true)
			return;

		var existTranslates = await dbContext.ListingTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.ListingId == @event.Listing.Id)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			foreach (var translate in @event.DescriptionTranslates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.ListingTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
					await dbContext.ListingTranslates.AddAsync(new ListingTranslate(@event.Listing.Id, translate.LanguageId, translate.LanguageShortCode, translate.Value), cancellationToken);
			}
		}
		else
		{
			await dbContext.ListingTranslates.AddRangeAsync(@event
					.DescriptionTranslates
					.Select(item => new ListingTranslate(
						@event.Listing.Id,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value
					)), cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}

