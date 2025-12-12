using Domain;

namespace Application;

public class CreateCharacter(ICharacterService characterService) : ICreateCharacter
{
    public async Task<CharacterId> Execute(CreateCharacterRequest request)
    {
        CreateCharacterCommand command = request.characterClass switch
        {
            CharacterClass.Tactician => new CreateTacticianCommand(request.Name),
            CharacterClass.Fury => new CreateFuryCommand(request.Name),
            _ => throw new ArgumentException("Invalid character class", nameof(request.characterClass))
        };

        return await characterService.Create(command);
    }
}