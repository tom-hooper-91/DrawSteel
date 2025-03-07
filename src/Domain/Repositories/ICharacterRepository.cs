namespace Domain.Repositories;

public interface ICharacterRepository
{
    Task<CharacterId> Add(Character character);
    Task<Character> Get(CharacterId id);
}