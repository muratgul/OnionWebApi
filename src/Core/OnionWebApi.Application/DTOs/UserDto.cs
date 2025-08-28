namespace OnionWebApi.Application.DTOs;
public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
}
