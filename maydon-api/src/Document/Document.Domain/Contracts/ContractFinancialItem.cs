using System.ComponentModel.DataAnnotations;
using Core.Domain.Entities;
using Core.Domain.ValueObjects;
using Document.Domain.Contracts.Enums;

namespace Document.Domain.Contracts;

public sealed class ContractFinancialItem : EntityBase<Guid>
{
    public Guid ContractId { get; private set; }
    public FinancialItemType Type { get; private set; }

    [MaxLength(256)]
    public string Name { get; private set; } = string.Empty;
    public Money Amount { get; private set; }
    public FinancialFrequency Frequency { get; private set; }
    public int SortOrder { get; private set; }

    private ContractFinancialItem() { } // EF Core

    public ContractFinancialItem(
        Guid contractId,
        FinancialItemType type,
        string name,
        Money amount,
        FinancialFrequency frequency,
        int sortOrder = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = Guid.CreateVersion7();
        ContractId = contractId;
        Type = type;
        Name = name;
        Amount = amount;
        Frequency = frequency;
        SortOrder = sortOrder;
    }

    public void Update(string name, Money amount, FinancialFrequency frequency, int sortOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Amount = amount;
        Frequency = frequency;
        SortOrder = sortOrder;
    }
}
