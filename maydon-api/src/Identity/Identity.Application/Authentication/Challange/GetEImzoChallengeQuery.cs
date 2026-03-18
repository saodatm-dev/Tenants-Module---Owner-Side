using Core.Application.Abstractions.Messaging;
using Identity.Application.Core.Abstractions.Services.EImzo;

namespace Identity.Application.Authentication.Challange;

public sealed record GetEImzoChallengeQuery : IQuery<ChallengeResponse>;
