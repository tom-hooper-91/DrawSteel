using Domain;

namespace Application;

public class CreateCharacter(ICharacterFactory factory, ISaveCharacter save) : ICreateCharacter
{
    public async Task<CharacterId> Execute(CreateCharacterCommand command)
    {
        var character = factory.Create(command);
        return await save.This(character);
    }
}