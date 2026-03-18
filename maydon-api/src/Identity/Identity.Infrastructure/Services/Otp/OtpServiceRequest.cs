using System.Text.Json.Serialization;

namespace Identity.Infrastructure.Services.Otp;

public sealed record OtpServiceRequest(
	[property: JsonPropertyName("messages")] List<Message> Messages);

public sealed record Message(
	[property: JsonPropertyName("recipient")] string Recipient,
	[property: JsonPropertyName("sms")] MessageSms Sms)
{

	[JsonPropertyName("message-id")] public string MessageId => DateTime.UtcNow.ToFileTime().ToString()[..12];
	//[JsonPropertyName("priority")] public string Priority => string.Empty;
}

public sealed record MessageSms(
	[property: JsonPropertyName("content")] MessageSmsContent Content)
{
	[JsonPropertyName("originator")] public string Originator => "MAYDON";
}

public sealed record MessageSmsContent([property: JsonPropertyName("text")] string Text);
