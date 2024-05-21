using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace todo_api_app.Dtos;

public record class GenerateTokenRequestDto
{
    [Required]
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }

    [Required]
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    public GenerateTokenRequestDto(string refreshToken, string accessToken)
    {
        RefreshToken = refreshToken;
        AccessToken = accessToken;
    }
}
