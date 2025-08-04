namespace OnionWebApi.Application.DTOs;
public record ChangePasswordRequestDto(int userId, string oldPassword, string newPassword);
