using System.Reflection.PortableExecutable;
using Domain;
using Domain.Repositories;

namespace Infrastructure;

public class InMemoryCharacterRepository : ICharacterRepository
{
    private List<Character> _characters = [];

    public CharacterId Add(Character character)
    {
        _characters.Add(character);
        return character.Id;
    }

    public Character Get(CharacterId id)
    {
        return _characters.Single(character => character.Id == id);
    }
}