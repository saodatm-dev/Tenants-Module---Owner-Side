using Common.Domain.Banks;
using Common.Domain.Currencies;
using Common.Domain.Districts;
using Common.Domain.Languages;
using Common.Domain.Regions;
using Core.Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Core.Abstractions.Data;

public interface ICommonDbContext : IApplicationDbContext
{
	DbSet<Bank> Banks { get; }
	DbSet<BankTranslate> BankTranslates { get; }
	DbSet<Currency> Currencies { get; }
	DbSet<CurrencyTranslate> CurrencyTranslates { get; }
	DbSet<District> Districts { get; }
	DbSet<DistrictTranslate> DistrictTranslates { get; }
	DbSet<Language> Languages { get; }
	DbSet<Region> Regions { get; }
	DbSet<RegionTranslate> RegionTranslates { get; }
}
