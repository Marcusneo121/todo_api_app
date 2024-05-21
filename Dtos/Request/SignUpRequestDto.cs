using System.ComponentModel.DataAnnotations;

namespace todo_api_app.Dtos;

public record class SignUpRequestDto(
    [Required][StringLength(50)] string Name,
    [Required][StringLength(50)] string Email,
    [Required][StringLength(12)] string Password
);
