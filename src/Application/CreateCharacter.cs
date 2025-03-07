using Domain;

namespace Application;

public class CreateCharacter(ICharacterFactory characterFactory, ISaveCharacter saveCharacter) : ICreateCharacter
{
    public async Task<CharacterId> Execute(CreateCharacterCommand command)
    {
        var character = characterFactory.Create(command);
        return await saveCharacter.Save(character);
    }
}