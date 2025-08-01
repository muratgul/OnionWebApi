namespace OnionWebApi.Application.Services;
public class OtpService : IOtpService
{
    public string GenerateOtpAuthUri(string account, string issuer, string secretKey)
    {        
        return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(account)}?secret={secretKey}&issuer={Uri.EscapeDataString(issuer)}&digits=6";
    }
    public byte[] GenerateQrCode(string account, string issuer, string key)
    {
        var otpauthUrl = GenerateOtpAuthUri(account, issuer, key);
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(otpauthUrl, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }
    public async Task<byte[]> GenerateQrCodeAsync(string account, string issuer, string key)
    {
        return await Task.Run(() => GenerateQrCode(account, issuer, key));
    }
    public (byte[], string) GenerateRandomKey(int keySize = 20)
    {
        var secretKey = KeyGeneration.GenerateRandomKey(keySize);
        var base32Secret = Base32Encoding.ToString(secretKey);
        return (secretKey, base32Secret);
    }

    public bool ValidateOtp(byte[] secretKey, string otpCode)
    {
        if (secretKey == null || secretKey.Length == 0)
            return false;

        if (string.IsNullOrWhiteSpace(otpCode) || !otpCode.All(char.IsDigit) || otpCode.Length != 6)
            return false;

        var totp = new Totp(secretKey);
        return totp.VerifyTotp(otpCode, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }

    public OtpSetupResult FirstCode(string issuer, string account)
    {
        var (secretKeyBytes, base32Secret) = GenerateRandomKey();
        var otpAuthUrl = GenerateOtpAuthUri(account, issuer, base32Secret);
        var qrCodeBytes = GenerateQrCode(account, issuer, base32Secret);
        var qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);

        return new OtpSetupResult
        {
            SecretKeyBytes = secretKeyBytes,
            Base32Secret = base32Secret,
            QrCodeBase64 = qrCodeBase64,
            OtpAuthUrl = otpAuthUrl
        };
    }

}
