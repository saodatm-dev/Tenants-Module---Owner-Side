namespace Core.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount in UZS (Uzbek So'm).
/// Stored internally as decimal (so'm with 2 decimal places).
/// 
/// This is the ONLY way money should be represented in the domain.
/// No raw long/decimal for money anywhere — always use Money.
/// </summary>
public readonly record struct Money : IComparable<Money>
{
    /// <summary>
    /// Amount in so'm (e.g., 15_000_000.50).
    /// </summary>
    public decimal Amount { get; }

    private Money(decimal amount) => Amount = Math.Round(amount, 2, MidpointRounding.ToEven);

    // ─── Factory methods ───

    /// <summary>Creates Money from so'm (e.g., 15_000_000 or 15_000_000.50).</summary>
    public static Money FromSom(decimal som) => new(som);

    /// <summary>Creates Money from tiyin (smallest unit, 1 so'm = 100 tiyin).</summary>
    public static Money FromTiyin(long tiyin) => new(tiyin / 100m);

    // ─── Internal precision ───

    /// <summary>
    /// Amount in tiyin for precise backend calculations.
    /// NOT exposed to API clients.
    /// </summary>
    public long Tiyin => (long)(Amount * 100m);

    /// <summary>Returns whole so'm, dropping tiyin remainder.</summary>
    public long ToWholeSom() => (long)Amount;

    /// <summary>Returns just the tiyin remainder (0-99).</summary>
    public int TiyinRemainder => (int)(Math.Abs(Tiyin) % 100);

    // ─── Arithmetic ───

    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);
    public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount);
    public static Money operator *(Money a, int multiplier) => new(a.Amount * multiplier);
    public static Money operator *(Money a, decimal multiplier) => new(a.Amount * multiplier);

    /// <summary>
    /// Divide money into N equal parts WITH remainder handling.
    /// Allocate(3) on 100.00 so'm → [33.34, 33.33, 33.33] — no money is ever lost.
    /// </summary>
    public Money[] Allocate(int parts)
    {
        if (parts <= 0) throw new ArgumentException("Parts must be > 0");

        var baseAmount = Math.Floor(Amount * 100m / parts) / 100m;
        var remainder = Amount - baseAmount * parts;

        var result = new Money[parts];
        var remainderTiyin = (int)(remainder * 100m);

        for (var i = 0; i < parts; i++)
        {
            result[i] = new Money(baseAmount + (i < remainderTiyin ? 0.01m : 0m));
        }

        return result;
    }

    /// <summary>
    /// Calculate percentage with proper rounding.
    /// Money.FromSom(15_000_000).Percent(0.5m) → penalty per day
    /// </summary>
    public Money Percent(decimal percentage) => new(Amount * percentage / 100m);

    // ─── Comparison ───

    public int CompareTo(Money other) => Amount.CompareTo(other.Amount);
    public static bool operator >(Money a, Money b) => a.Amount > b.Amount;
    public static bool operator <(Money a, Money b) => a.Amount < b.Amount;
    public static bool operator >=(Money a, Money b) => a.Amount >= b.Amount;
    public static bool operator <=(Money a, Money b) => a.Amount <= b.Amount;

    public static readonly Money Zero = new(0m);

    public override string ToString() => Amount.ToString("N2");
}
