using Domain;

namespace Application;

public class CreateCharacter(ICharacterService characterService) : ICreateCharacter
{
    public async Task<CharacterId> Execute(CreateCharacterInput request)
    {
        var command = new CreateCharacterCommand(request.Name);
        return await characterService.Create(command);
    }
}