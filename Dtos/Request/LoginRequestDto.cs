using System.ComponentModel.DataAnnotations;

namespace todo_api_app.Dtos;

public record class LoginRequestDto(
    [Required][StringLength(50)] string Email,
    [Required][StringLength(12)] string Password
);
