using KhKhDoumi.Encryption;
using Core.Application.Abstractions.Security;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Security;

/// <summary>
/// String encryption using KhKhDoumi AES encryption scheme.
/// Delegates to the KhKhDoumi library for symmetric key encryption.
/// </summary>
public sealed class KhKhDoumiEncryptor(IOptionsMonitor<EncryptionOptions> optionsMonitor) : IStringEncryptor
{
    private readonly IOptionsMonitor<EncryptionOptions> _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
    private readonly EncryptionScheme _scheme = EncryptionScheme.AES;

    public string Encrypt(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext)) return plaintext;
        var passKey = _optionsMonitor.CurrentValue.PassKey;
        return new EnDec(passKey).Encrypt(plaintext, _scheme);
    }

    public string Decrypt(string ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext)) return ciphertext;
        var passKey = _optionsMonitor.CurrentValue.PassKey;
        return new EnDec(passKey).Decrypt(ciphertext, _scheme);
    }
}
