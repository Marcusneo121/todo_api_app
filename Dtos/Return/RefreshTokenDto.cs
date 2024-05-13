using System.ComponentModel.DataAnnotations;

namespace todo_api_app.Dtos;

public record class RefreshTokenDto(
    [Required] string RefreshToken,
    [Required] DateTime ExpiredDate
);
