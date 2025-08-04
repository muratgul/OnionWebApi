namespace OnionWebApi.Application.DTOs;
public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
}
