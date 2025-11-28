using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Requests;

public sealed class CharacterRequest
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [Required(ErrorMessage = "The 'name' field is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "The 'name' field must be between 1 and 100 characters.")]
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
