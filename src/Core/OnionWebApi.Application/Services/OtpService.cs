namespace OnionWebApi.Application.Services;
public class OtpService : IOtpService
{
    private const int DefaultKeySize = 20;
    private const int OtpCodeLength = 6;
    private const int QrCodePixelSize = 20;
    public string GenerateOtpAuthUri(string account, string issuer, string secretKey)
    {
        ValidateOtpAuthParameters(account, issuer, secretKey);

        var encodedIssuer = Uri.EscapeDataString(issuer);
        var encodedAccount = Uri.EscapeDataString(account);

        return $"otpauth://totp/{encodedIssuer}:{encodedAccount}?secret={secretKey}&issuer={encodedIssuer}&digits={OtpCodeLength}";
    }
    public byte[] GenerateQrCode(string account, string issuer, string key)
    {
        var otpauthUrl = GenerateOtpAuthUri(account, issuer, key);
        return CreateQrCodeImage(otpauthUrl);
    }
    public async Task<byte[]> GenerateQrCodeAsync(string account, string issuer, string key)
    {
        return await Task.Run(() => GenerateQrCode(account, issuer, key), CancellationToken.None).ConfigureAwait(false);
    }
    public (byte[], string) GenerateRandomKey(int keySize = DefaultKeySize)
    {
        ValidateKeySize(keySize);

        var secretKey = KeyGeneration.GenerateRandomKey(keySize);
        var base32Secret = Base32Encoding.ToString(secretKey);

        return (secretKey, base32Secret);
    }
    public bool ValidateOtp(byte[] secretKey, string otpCode)
    {
        if (!IsValidSecretKey(secretKey) || !IsValidOtpCode(otpCode))
        {
            Log.Warning("OTP validation failed: Invalid secret or OTP code.");
            return false;
        }

        var totp = new Totp(secretKey);
        var isValid = totp.VerifyTotp(otpCode, out _, new VerificationWindow(1, 1));        
        return isValid;
    }
    public OtpSetupResult CreateOtpSetup(string issuer, string account)
    {
        var (secretKeyBytes, base32Secret) = GenerateRandomKey();
        var otpAuthUrl = GenerateOtpAuthUri(account, issuer, base32Secret);
        var qrCodeBytes = GenerateQrCode(account, issuer, base32Secret);
        var qrCodeBase64 = "data:image/png;base64," + Convert.ToBase64String(qrCodeBytes);

        return new OtpSetupResult
        {
            SecretKeyBytes = secretKeyBytes,
            Base32Secret = base32Secret,
            QrCodeBase64 = qrCodeBase64,
            OtpAuthUrl = otpAuthUrl
        };
    }

    private static void ValidateOtpAuthParameters(string account, string issuer, string secretKey)
    {
        if (string.IsNullOrWhiteSpace(account))
        {
            throw new ArgumentException("Account cannot be null or empty.", nameof(account));
        }

        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new ArgumentException("Issuer cannot be null or empty.", nameof(issuer));
        }

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new ArgumentException("Secret key cannot be null or empty.", nameof(secretKey));
        }
    }
    private static byte[] CreateQrCodeImage(string otpauthUrl)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(otpauthUrl, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(QrCodePixelSize);
    }
    private static void ValidateKeySize(int keySize)
    {
        if (keySize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(keySize), "Key size must be greater than zero.");
        }

        if (keySize < 10)
        {
            throw new ArgumentOutOfRangeException(nameof(keySize), "Key size should be at least 10 bytes for security.");
        }
    }
    private static bool IsValidSecretKey(byte[] secretKey)
    {
        return secretKey != null && secretKey.Length > 0;
    }
    private static bool IsValidOtpCode(string otpCode)
    {
        return !string.IsNullOrWhiteSpace(otpCode)
               && otpCode.Length == OtpCodeLength
               && otpCode.All(char.IsDigit);
    }
}
