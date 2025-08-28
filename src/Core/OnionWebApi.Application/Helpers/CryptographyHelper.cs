namespace OnionWebApi.Application.Helpers;
public static class CryptographyHelper
{
    private static readonly string DefaultKey = "anks54r6w465a46c4656s5464a6s8ewe";
    private static readonly string DefaultIV = "kjh6kjnksd987nsb";

    // <summary>
    /// Metni AES algoritması ile şifreler
    /// </summary>
    /// <param name="plainText">Şifrelenecek metin</param>
    /// <param name="key">Şifreleme anahtarı (opsiyonel)</param>
    /// <returns>Base64 formatında şifreli metin</returns>
    public static string Encrypt(string plainText, string key = "")
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return string.Empty;
        }

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key ?? DefaultKey);
        aes.IV = Encoding.UTF8.GetBytes(DefaultIV);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using var swEncrypt = new StreamWriter(csEncrypt);
            swEncrypt.Write(plainText);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    /// <summary>
    /// Şifreli metni AES algoritması ile çözer
    /// </summary>
    /// <param name="cipherText">Base64 formatında şifreli metin</param>
    /// <param name="key">Şifreleme anahtarı (opsiyonel)</param>
    /// <returns>Düz metin</returns>
    public static string Decrypt(string chipperText, string key = "")
    {
        if (string.IsNullOrEmpty(chipperText))
        {
            return string.Empty;
        }

        try
        {
            var cipherBytes = Convert.FromBase64String(chipperText);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key ?? DefaultKey);
            aes.IV = Encoding.UTF8.GetBytes(DefaultIV);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(cipherBytes);
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        catch (CryptographicException)
        {
            return string.Empty; // Return empty string if decryption fails
        }
        catch (FormatException)
        {
            return string.Empty; // Return empty string if format is invalid
        }
    }


    /// <summary>
    /// Rastgele anahtar oluşturur (16 karakter)
    /// </summary>
    /// <returns>16 karakterlik rastgele anahtar</returns>
    public static string GenerateRandomKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[16];
        return Convert.ToBase64String(randomBytes).Substring(0, 16);
    }


    /// <summary>
    /// Güvenli şifreleme - her seferinde farklı IV kullanır
    /// </summary>
    /// <param name="plainText">Şifrelenecek metin</param>
    /// <param name="key">Şifreleme anahtarı</param>
    /// <returns>IV + şifreli metin birleşik Base64 string</returns>
    public static string EncrypySecure(string plainText, string key = "")
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return string.Empty;
        }

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key ?? DefaultKey);
        aes.GenerateIV();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        msEncrypt.Write(aes.IV, 0, aes.IV.Length);

        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);
        swEncrypt.Write(plainText);
        return Convert.ToBase64String(msEncrypt.ToArray());
    }


    /// <summary>
    /// Güvenli şifre çözme - IV'yi şifreli metinden ayırır
    /// </summary>
    /// <param name="cipherText">IV + şifreli metin birleşik Base64 string</param>
    /// <param name="key">Şifreleme anahtarı</param>
    /// <returns>Düz metin</returns>
    public static string DecryptSecure(string cipherText, string key = "")
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key ?? DefaultKey);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var cipher = new byte[fullCipher.Length - iv.Length];
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        catch (Exception)
        {
            return string.Empty; // Return empty string if decryption fails
        }
    }
}
