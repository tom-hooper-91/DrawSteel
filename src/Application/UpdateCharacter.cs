using System;
using Domain;

namespace Application;

public class UpdateCharacter(ICharacterService characterService) : IUpdateCharacter
{
    public async Task<CharacterDto?> Execute(UpdateCharacterInput request)
    {
        var identifier = ResolveIdentifier(request);
        var command = new UpdateCharacterCommand(new CharacterId(identifier), request.Name);
        var character = await characterService.Update(command);
        return character is null ? null : CharacterDto.FromCharacter(character);
    }

    private static Guid ResolveIdentifier(UpdateCharacterInput request)
    {
        if (string.IsNullOrWhiteSpace(request.PayloadId))
        {
            throw new ArgumentException("The payload id is required for updates.", nameof(request.PayloadId));
        }

        if (!Guid.TryParse(request.PayloadId, out var payloadIdentifier))
        {
            throw new ArgumentException("The payload id must be a valid GUID.", nameof(request.PayloadId));
        }

        if (payloadIdentifier != request.RouteId)
        {
            throw new ArgumentException("The payload id must match the route identifier.", nameof(request.PayloadId));
        }

        return payloadIdentifier;
    }
}
