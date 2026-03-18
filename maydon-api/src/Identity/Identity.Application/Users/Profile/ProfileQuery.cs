using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Users.Profile;

public sealed record ProfileQuery : IQuery<ProfileResponse>;
