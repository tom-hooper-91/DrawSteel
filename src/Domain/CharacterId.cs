namespace Domain;

public record CharacterId(Guid Value)
{
    public override string ToString() => Value.ToString();
}