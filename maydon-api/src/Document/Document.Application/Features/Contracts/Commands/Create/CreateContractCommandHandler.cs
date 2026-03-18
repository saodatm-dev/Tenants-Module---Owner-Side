using ContractEntity = Document.Domain.Contracts.Contract;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services;
using Core.Application.Resources;
using Core.Domain.Enums;
using Core.Domain.Results;
using Core.Domain.ValueObjects;
using Document.Application.Abstractions.Data;
using Document.Application.Features.Contracts.Mappings;
using Document.Contract.Contracts;
using Document.Contract.Contracts.Commands;
using Document.Contract.Contracts.Responses;
using Document.Contract.Gateways;
using Document.Domain.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.Contracts.Commands.Create;

/// <summary>
/// Creates a new contract in Draft status by resolving lease/company data
/// and generating a contract number.
/// </summary>
public sealed class CreateContractCommandHandler(
    IDocumentDbContext dbContext,
    IBuildingReadGateway buildingGateway,
    IUserLookupService userLookupService,
    IContractNumberGenerator numberGenerator,
    IExecutionContextProvider executionContext,
    ISharedViewLocalizer sharedViewLocalizer,
    IVersioningService versioningService)
    : ICommandHandler<CreateContractCommand, ContractResponse>
{
    public async Task<Result<ContractResponse>> Handle(
        CreateContractCommand command,
        CancellationToken cancellationToken)
    {
        var tenantId = executionContext.TenantId;
        var userId = executionContext.UserId;

        // 1. Validate template exists
        var templateExists = await dbContext.ContractTemplates
            .AsNoTracking()
            .AnyAsync(t => t.Id == command.TemplateId && t.IsActive, cancellationToken);

        if (!templateExists)
            return Result.Failure<ContractResponse>(
                sharedViewLocalizer.TemplateNotFound(nameof(command.TemplateId)));

        // 2. Load lease data
        var lease = await buildingGateway.GetLeaseInfoAsync(command.LeaseId, cancellationToken);
        if (lease is null)
            return Result.Failure<ContractResponse>(sharedViewLocalizer.LeaseNotFound(nameof(command.LeaseId)));

        // 3. Resolve owner user data (INN/PINFL) from lease.OwnerId
        var ownerResult = await userLookupService.GetByIdAsync(lease.OwnerId, cancellationToken);
        var ownerInn = ownerResult.IsSuccess ? ownerResult.Value.Tin : null;
        var ownerPinfl = ownerResult.IsSuccess ? ownerResult.Value.Pinfl : null;

        // 4. Resolve client user data (INN/PINFL) from lease.ClientId
        var clientResult = await userLookupService.GetByIdAsync(lease.ClientId, cancellationToken);
        var clientInn = clientResult.IsSuccess ? clientResult.Value.Tin : null;
        var clientPinfl = clientResult.IsSuccess ? clientResult.Value.Pinfl : null;

        // 5. Resolve client company (if registered) via CompanyUsers
        var clientCompanyId = await userLookupService.GetCompanyIdByUserIdAsync(lease.ClientId, cancellationToken);

        // 6. Create the contract aggregate
        // Use first item's RealEstateId and aggregate financials
        var firstItem = lease.Items.First();
        var totalMonthlyRent = Money.FromSom(
            lease.Items.Sum(i => i.MonthlyRent.Amount));

        var contract = ContractEntity.Create(
            tenantId: tenantId,
            templateId: command.TemplateId,
            language: command.Language,
            body: command.Body,
            leaseId: lease.LeaseId,
            realEstateId: firstItem.RealEstateId,
            ownerCompanyId: tenantId,
            clientCompanyId: clientCompanyId,
            ownerInn: ownerInn,
            ownerPinfl: ownerPinfl,
            clientInn: clientInn,
            clientPinfl: clientPinfl,
            monthlyAmount: totalMonthlyRent,
            leaseStartDate: lease.StartDate,
            leaseEndDate: lease.EndDate,
            createdByUserId: userId);

        // 6. Generate contract number
        var contractNumber = await numberGenerator.GenerateNextAsync(tenantId, cancellationToken);
        contract.SetContractNumber(contractNumber);

        // 7. Add financial items per lease item
        var sortOrder = 1;
        foreach (var item in lease.Items)
        {
            contract.AddFinancialItem(
                FinancialItemType.Rent,
                "Арендная плата",
                item.MonthlyRent,
                FinancialFrequency.Monthly,
                sortOrder: sortOrder++);

            if (item.DepositAmount > Money.Zero)
            {
                contract.AddFinancialItem(
                    FinancialItemType.Deposit,
                    "Обеспечительный депозит",
                    item.DepositAmount,
                    FinancialFrequency.OneTime,
                    sortOrder: sortOrder++);
            }
        }

        contract.RaiseCreatedEvent();

        await dbContext.Contracts.AddAsync(contract, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await versioningService.PublishVersionSnapshotAsync(contract, EntityChangeType.Created, cancellationToken);

        return Result.Success(contract.ToResponse());
    }
}
