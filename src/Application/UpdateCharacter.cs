using Domain;

namespace Application;

public class UpdateCharacter(ICharacterService characterService) : IUpdateCharacter
{
    public async Task<Character?> Execute(UpdateCharacterCommand command)
    {
        return await characterService.Update(command);
    }
}
