namespace OnionWebApi.Application.Interfaces.Otp;
public class OtpSetupResult
{
    public byte[] SecretKeyBytes { get; set; }
    public string Base32Secret { get; set; }
    public string QrCodeBase64 { get; set; }
    public string OtpAuthUrl { get; set; }
}
