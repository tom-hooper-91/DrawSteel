namespace Domain.Repositories;

public interface ICharacterRepository
{
    CharacterId Add(Character character);
    Character Get(CharacterId id);
}