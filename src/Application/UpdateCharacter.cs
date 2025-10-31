using Domain;

namespace Application;

// T024: Create UpdateCharacter use case implementation
public class UpdateCharacter(ICharacterService characterService) : IUpdateCharacter
{
    public async Task<Character?> Execute(UpdateCharacterCommand command)
    {
        return await characterService.Update(command);
    }
}
