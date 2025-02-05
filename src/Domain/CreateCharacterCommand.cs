using System.Text.Json.Serialization;

namespace Domain;

public record CreateCharacterCommand(string Name);