using System.ComponentModel.DataAnnotations;

namespace todo_api_app.Dtos;

public record class AccessTokenRequestDto(
    [Required] int Id,
    [Required] string Name,
    [Required] string Email
);
