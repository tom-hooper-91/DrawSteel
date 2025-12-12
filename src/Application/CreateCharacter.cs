using Domain;

namespace Application;

public class CreateCharacter(ICharacterService characterService) : ICreateCharacter
{
    public async Task<CharacterId> Execute(CreateCharacterRequest request)
    {
        CreateCharacterCommand command = request.characterClass switch
        {
            CharacterClass.Warrior => new CreateWarriorCommand(request.Name),
            CharacterClass.Gardener => new CreateGardenerCommand(request.Name),
            _ => throw new ArgumentException("Invalid character class", nameof(request.characterClass))
        };

        return await characterService.Create(command);
    }
}