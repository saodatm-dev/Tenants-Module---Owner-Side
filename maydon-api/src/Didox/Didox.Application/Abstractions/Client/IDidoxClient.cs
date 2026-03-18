using Didox.Application.Abstractions.Client.Account;
using Didox.Application.Abstractions.Client.Document;
using Didox.Application.Abstractions.Client.Eimzo;
using Didox.Application.Abstractions.Client.JsonDocument;
using Didox.Application.Abstractions.Client.Login;
using Didox.Application.Abstractions.Client.Registration;

namespace Didox.Application.Abstractions.Client;

/// <summary>
/// Interface for the Didox HTTP client — responsible only for interacting with the Didox API.
/// </summary>
public interface IDidoxClient : IDidoxAccountClient, IDidoxRegistrationClient, IDidoxDocumentClient, IDidoxEimzoClient, IDidoxLoginClient, IDidoxJsonDocumentClient;
