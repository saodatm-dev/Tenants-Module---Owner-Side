using Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Core.Infrastructure.Converters;

/// <summary>
/// Stores Money as numeric(18,2) (so'm) in PostgreSQL.
/// </summary>
public sealed class MoneyToDecimalConverter() : 
    ValueConverter<Money, decimal>(money => money.Amount, amount => Money.FromSom(amount));
