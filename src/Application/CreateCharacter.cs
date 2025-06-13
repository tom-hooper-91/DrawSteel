using Domain;

namespace Application;

public class CreateCharacter(ICharacterFactory factory, ISaveCharacter save) : ICreateCharacter
{
    public async Task<CharacterId> Execute(CreateCharacterCommand newCharacter)
    {
        var character = factory.Create(newCharacter);
        return await save.This(character);
    }
}