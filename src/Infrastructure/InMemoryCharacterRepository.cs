using Domain;
using Domain.Repositories;

namespace Infrastructure;

public class InMemoryCharacterRepository : ICharacterRepository
{
    private List<Character> _characters = [];

    public async Task<CharacterId> Add(Character character)
    {
        _characters.Add(character);
        await Task.Delay(1);
        return character.Id;
    }

    public async Task<Character> Get(CharacterId id)
    {
        await Task.Delay(1);
        return _characters.Single(character => character.Id == id);
    }
}