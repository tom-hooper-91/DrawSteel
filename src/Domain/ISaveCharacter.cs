namespace Domain;

public interface ISaveCharacter
{
    Task<CharacterId> Save(Character character);
}