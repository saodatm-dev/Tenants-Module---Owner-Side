using Document.Contract.ContractTemplates.Queries;
using FluentValidation;

namespace Document.Application.Features.ContractTemplates.Queries.GetList;

public sealed class GetContractTemplatesQueryValidator : AbstractValidator<GetContractTemplatesQuery>
{
    public GetContractTemplatesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        When(x => x.Scope.HasValue, () =>
        {
            RuleFor(x => x.Scope!.Value)
                .IsInEnum().WithMessage("Invalid scope value.");
        });

        When(x => x.Category.HasValue, () =>
        {
            RuleFor(x => x.Category!.Value)
                .IsInEnum().WithMessage("Invalid category value.");
        });
    }
}
