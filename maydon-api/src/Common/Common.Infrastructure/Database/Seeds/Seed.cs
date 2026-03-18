using Common.Application.Core.Abstractions.Data;
using Common.Domain.Districts;
using Common.Domain.Languages;
using Common.Domain.Regions;
using Core.Application.Resources;
using Core.Domain.Languages;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Database.Seeds;

internal static class Seed
{
	#region Languages

	private const string UzbekLanguageName = "O'zbek";
	private const string RussianLanguageName = "Русский";
	private const string EnglishLanguageName = "English";
	private const string UzTashkent = "Toshkent";
	private const string UzKarakalpakstan = "Qoraqalpog’iston Respublikasi";
	private const string UzAndijan = "Andijon viloyati";
	private const string UzBukhara = "Buxoro viloyati";
	private const string UzJizzakh = "Jizzax viloyati";
	private const string UzKashkadarya = "Qashqadaryo viloyati";
	private const string UzNavoiy = "Navoiy viloyati";
	private const string UzNamangan = "Namangan viloyati";
	private const string UzSamarkand = "Samarqand viloyati";
	private const string UzSurkhondaryo = "Surxondaryo viloyati";
	private const string UzSirdaryo = "Sirdaryo viloyati";
	private const string UzTashkentRegion = "Toshkent viloyati";
	private const string UzFergana = "Farg’ona viloyati";
	private const string UzKhorezm = "Xorazm viloyati";

	private static List<Language> Languages =>
		[new Language(UzbekLanguageName, ISharedViewLocalizer.UzbekTwoLetter),
		new Language(RussianLanguageName, ISharedViewLocalizer.RussianTwoLetter),
		new Language(EnglishLanguageName, ISharedViewLocalizer.EnglishTwoLetter) ];

	#endregion

	#region Regions

	private static List<Region> Regions(List<Language> languages)
	{
		var uzLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.UzbekTwoLetter)!;
		var ruLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.RussianTwoLetter)!;
		var enLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.EnglishTwoLetter)!;

		return [
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzTashkent),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Ташкент"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Tashkent")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzKarakalpakstan),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Республика Каракалпакстан"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Republic of Karakalpakstan")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzAndijan),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Андижанская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Andijan Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzBukhara),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Бухарская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Bukhara Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzJizzakh),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Джизакская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Jizzakh Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzKashkadarya),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Кашкадарьинская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Kashkadarya Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzNavoiy),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Навоийская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Navoiy Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzNamangan),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Наманганская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Namangan Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzSamarkand),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Самаркандская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Samarkand Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzSurkhondaryo),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Сурхандарьинская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Surkhondaryo Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzSirdaryo),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Сырдарьинская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Sirdaryo Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzTashkentRegion),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Ташкентская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Tashkent Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzFergana),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Ферганская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Fergana Region")]),
			new Region([
				new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,UzKhorezm),
				new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Хорезмская область"),
				new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Khorezm Region")])];
	}

	#endregion

	#region Districts
	private static List<District> Districts(
		List<Language> languages,
		List<Region> regions)
	{
		var uzLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.UzbekTwoLetter)!;
		var ruLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.RussianTwoLetter)!;
		var enLanguage = languages.Find(item => item.ShortCode == ISharedViewLocalizer.EnglishTwoLetter)!;

		var districts = new List<District>();

		// Tashkent
		var tashkentId = regions.Find(item => item.LanguageValues.Any(l => l.Value == UzTashkent))!.Id;

		districts.AddRange(
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Bektemir"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Бектемир"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Bektemir")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Chilonzor"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Чиланзар"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Chilanzar")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Yashnobod"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Яшнабад"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Yashnobod")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Mirobod"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Мирабад"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Mirobod")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Mirzo Ulug'bek"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Мирза Улугбек"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Mirzo Ulugbek")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Sirg'ali"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Сергели"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Sergeli")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Shayxontohur"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Шайхантахур"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Shayxontoxur")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Olmazor"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Алмазар"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Olmazor")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Uchtepa"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Учтепа"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Uchtepa")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Yakkasaroy"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Яккасарай"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Yakkasaray")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Yunusobod"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Юнусабад"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Yunusabad")]),
			new District(
				tashkentId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Yangihayot"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Янгихаят"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Yangihayot")]));

		// Tashkent region
		var tashkentRegionId = regions.Find(item => item.LanguageValues.Any(l => l.Value == UzTashkentRegion))!.Id;

		districts.AddRange(
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Bekobod"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Бекабад"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Bekabad")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Boʻstonliq"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Бустонлик"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Boʻstonliq")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Boʻka"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Бука"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Boʻka")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Chinoz"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Чиназ"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Chinoz")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Qibray"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Кибрай"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Qibray")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Ohangaron"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Ахангарон"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Ohangaron")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Oqqoʻrgʻon"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Аккурган"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Oqqoʻrgʻon")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Parkent"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Паркент"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Parkent")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Piskent"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Пскент"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Piskent")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Quyichirchiq"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Куйичирчик"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Quyichirchiq")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Zangiota"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Зангиата"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Zangiota")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Oʻrtachirchiq"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Уртачирчик"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Oʻrtachirchiq")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Yangiyoʻl"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Янгиюль"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Yangiyoʻl")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Yuqorichirchiq"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Юкарычирчик"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Yuqorichirchiq")]),
			new District(
				tashkentRegionId,
				[
					new LanguageValue(uzLanguage.Id,uzLanguage.ShortCode,"Toshkent tumani"),
					new LanguageValue(ruLanguage.Id,ruLanguage.ShortCode,"Ташкентский район"),
					new LanguageValue(enLanguage.Id,enLanguage.ShortCode,"Tashkent region")]));

		return districts;
	}
	#endregion

	public static async Task SeedingAsync(IServiceProvider serviceProvider, ICommonDbContext dbContext, CancellationToken cancellationToken)
	{
		var hasNewData = false;

		#region Languages
		var languages = Seed.Languages;
		if (!await dbContext.Languages.AnyAsync(cancellationToken))
		{
			await dbContext.Languages.AddRangeAsync(languages, cancellationToken);
			hasNewData = true;
		}
		else
		{
			var dbLanguages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);
			var exceptLanguages = languages.ExceptBy(dbLanguages.Select(item => item.ShortCode), item => item.ShortCode).ToList();

			if (exceptLanguages.Any())
				await dbContext.Languages.AddRangeAsync(exceptLanguages, cancellationToken);
		}
		#endregion

		#region Regions
		var regions = Seed.Regions(languages);
		if (!await dbContext.Regions.AnyAsync(cancellationToken))
		{
			await dbContext.Regions.AddRangeAsync(regions, cancellationToken);
			hasNewData = true;
		}
		else
		{
			var dbRegions = await dbContext.Regions.AsNoTracking().Include(item => item.Translates).ToListAsync(cancellationToken);
			var exceptRegions = regions.ExceptBy(
				dbRegions.Select(
					item => item.Translates.First().Value),
					item => item.LanguageValues.First(l => l.LanguageShortCode == ISharedViewLocalizer.UzbekTwoLetter).Value).ToList();

			if (exceptRegions.Any())
				await dbContext.Regions.AddRangeAsync(exceptRegions, cancellationToken);
		}
		#endregion

		#region Districts
		var districts = Seed.Districts(languages, regions);
		if (!await dbContext.Districts.AnyAsync(cancellationToken))
		{
			await dbContext.Districts.AddRangeAsync(districts, cancellationToken);
			hasNewData = true;
		}
		else
		{
			var dbDistricts = await dbContext.Districts.AsNoTracking().Include(item => item.Translates).ToListAsync(cancellationToken);
			districts = districts.ExceptBy(
				dbDistricts.Select(
					item => item.Translates.First().Value),
					item => item.LanguageValues.First(l => l.LanguageShortCode == ISharedViewLocalizer.UzbekTwoLetter).Value).ToList();

			if (districts.Any())
				await dbContext.Districts.AddRangeAsync(districts, cancellationToken);

		}
		#endregion
		if (hasNewData)
			await dbContext.SaveChangesAsync(cancellationToken);
	}
}

