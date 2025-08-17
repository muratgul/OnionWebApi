using System.Text.Json.Serialization;

namespace OnionWebApi.Domain.Dtos;

public class GetAccessTokenResponseDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
}
