using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace todo_api_app.Dtos;

public record class TodoDto
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [Required]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }
}