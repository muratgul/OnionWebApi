namespace OnionWebApi.Application.Interfaces.Otp;
public interface IOtpService
{
    (byte[], string) GenerateRandomKey(int keySize = 20);
    string GenerateOtpAuthUri(string account, string issuer, string secretKey);
    byte[] GenerateQrCode(string account, string issuer, string key);
    Task<byte[]> GenerateQrCodeAsync(string account, string issuer, string key);
    bool ValidateOtp(byte[] secretKey, string otpCode);
    OtpSetupResult FirstCode(string issuer, string account);
}
