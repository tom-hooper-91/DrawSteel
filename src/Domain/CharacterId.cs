using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public record CharacterId(Guid Value)
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Value { get; init; } = Value;
    public override string ToString() => Value.ToString();
}