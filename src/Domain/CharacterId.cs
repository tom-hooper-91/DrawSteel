using System.Text.Json.Serialization;

namespace Domain;

public record CharacterId(Guid Value)
{
    public override string ToString() => Value.ToString();
}