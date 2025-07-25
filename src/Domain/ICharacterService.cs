namespace Domain;

public interface ICharacterService
{
    Task<CharacterId> Create(CreateCharacterCommand character);
    Task<Character> Get(CharacterId characterId);
}