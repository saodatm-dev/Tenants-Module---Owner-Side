using System.Text.Json.Serialization;

namespace Identity.Application.Core.Abstractions.Services.EImzo;

public sealed record ChallengeResponse(
 [property: JsonPropertyName("challenge")] string Id,  // случайного значение которое пользоваетль должен будет подписать и создать EImzo#7 документ с помощю E-IMZO.
 [property: JsonPropertyName("ttl")] int TTL,   //время жизни challenge в секундах.
 [property: JsonPropertyName("status")] int status,   //код состояния (1 - Успешно, иначе ошибка)
 [property: JsonPropertyName("message")] string Message); //если status не равно 1, то сообщения об ошибки.
