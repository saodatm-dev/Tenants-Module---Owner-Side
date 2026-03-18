using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Application.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Cryptors;

public sealed class Cryptor(
    ILogger<Cryptor> logger,
    IOptionsMonitor<ApplicationOptions> applicationOptions) : ICryptor
{
    private string Encrypt(string key, string plainText)
    {
        try
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        catch (Exception ex)
        {
            logger.LogError("Cryptor's encrypt is throwing exception {Message}", ex.Message);
            return string.Empty;
        }
    }
    private string Decrypt(string key, string encryptedText)
    {
        try
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Cryptor's decrypt is throwing exception {Message}", ex.Message);
            return string.Empty;
        }
    }
    public string EncryptInvitation(Guid id) =>
        Uri.EscapeDataString(this.Encrypt(applicationOptions.CurrentValue.InvitationKey, $"{id}"));
    public Guid DecryptInvitation(string key)
    {
        var decryptedValue = Decrypt(applicationOptions.CurrentValue.InvitationKey, Uri.UnescapeDataString(key.Trim()));

        if (Guid.TryParse(decryptedValue, out var invitationId))
            return invitationId;

        return Guid.Empty;
    }

    private const string AccountSplitter = "__#__";
    public string EncryptAccount(Guid accountId, Guid sessionId) =>
        Uri.EscapeDataString(this.Encrypt(applicationOptions.CurrentValue.AccountKey, $"{accountId}{AccountSplitter}{sessionId}"));

    public (Guid AccountId, Guid SessionId)? DecryptAccount(string key)
    {
        var decryptedValue = this.Decrypt(applicationOptions.CurrentValue.AccountKey, Uri.UnescapeDataString(key.Trim()));
        if (string.IsNullOrWhiteSpace(decryptedValue))
            return null;

        var accountSession = decryptedValue.Split(AccountSplitter);

        // check values
        if (accountSession.Length != 2)
            return null;

        if (!Guid.TryParse(accountSession[0], out var accountId) || !Guid.TryParse(accountSession[1], out var sessionId))
            return null;

        return (accountId, sessionId);
    }

    public string EncryptUserState(Guid id) =>
        Uri.EscapeDataString(this.Encrypt(applicationOptions.CurrentValue.UserStateKey, $"{id}"));

    public Guid? DecryptUserState(string key)
    {
        var decryptedValue = this.Decrypt(applicationOptions.CurrentValue.UserStateKey, Uri.UnescapeDataString(key.Trim()));
        if (string.IsNullOrWhiteSpace(decryptedValue))
            return null;

        if (!Guid.TryParse(decryptedValue, out var userStateId))
            return null;

        return userStateId;
    }
}
