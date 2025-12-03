using Domain;

namespace Application;

public class CreateCharacter(ICharacterService characterService) : ICreateCharacter
{
    public async Task<CharacterId> Execute(CreateCharacterRequest request)
    {
        var command = new CreateWarriorCommand(request.Name);
        
        return await characterService.Create(command);
    }
}