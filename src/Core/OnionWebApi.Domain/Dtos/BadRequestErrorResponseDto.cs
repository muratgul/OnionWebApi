using System.Text.Json.Serialization;

namespace OnionWebApi.Domain.Dtos;

public class BadRequestErrorResponseDto
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = default!;
    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; set; } = default!;
}
