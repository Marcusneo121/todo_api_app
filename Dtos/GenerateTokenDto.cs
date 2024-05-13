using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace todo_api_app.Dtos;

public record class GenerateTokenDto
{
    [Required]
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }

    [Required]
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    public GenerateTokenDto(string refreshToken, string accessToken)
    {
        RefreshToken = refreshToken;
        AccessToken = accessToken;
    }
}
