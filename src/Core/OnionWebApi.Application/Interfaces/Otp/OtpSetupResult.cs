namespace OnionWebApi.Application.Interfaces.Otp;
public class OtpSetupResult
{
    public byte[] SecretKeyBytes { get; set; } = default!;
    public string Base32Secret { get; set; } = default!;
    public string QrCodeBase64 { get; set; } = default!;
    public string OtpAuthUrl { get; set; } = default!;
}
