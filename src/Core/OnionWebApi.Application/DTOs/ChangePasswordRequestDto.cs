namespace OnionWebApi.Application.DTOs;
public record ChangePasswordRequestDto(int UserId, string OldPassword, string NewPassword);
