namespace OnionWebApi.Application.DTOs.Email;

public class BulkEmailRequestDto
{
    public List<SendEmailRequestDto> Emails { get; set; } = [];
}