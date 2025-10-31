namespace Domain.Repositories;

public interface ICharacterRepository
{
    Task<CharacterId> Add(Character character);
    Task<Character> Get(CharacterId id);
    Task<bool> Update(Character character);
    Task<bool> Delete(CharacterId id);
}