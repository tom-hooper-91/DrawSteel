using Domain;

namespace Application;

public class CreateCharacter(ICharacterService characterService) : ICreateCharacter
{
    public async Task<CharacterId> Execute(CreateCharacterCommand command)
    {
        return await characterService.Create(command);
    }
}