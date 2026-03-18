using Core.Application.Abstractions.Messaging;

namespace Document.Contract.ContractTemplates.Commands;

public sealed record DeleteContractTemplateCommand(Guid Id) : ICommand;
