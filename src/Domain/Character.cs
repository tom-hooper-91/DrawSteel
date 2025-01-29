namespace Domain;

public record Character(string Name)
{
    public CharacterId Id { get; } = new(Guid.NewGuid());
}