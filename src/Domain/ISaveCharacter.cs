namespace Domain;

public interface ISaveCharacter
{
    Task<CharacterId> This(Character character);
}