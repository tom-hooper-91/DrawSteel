using System;
using API.Contracts;
using API.Diagnostics;
using API.Requests;
using API.Validation;
using Application;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API;

[ApiController]
[Route("api/[controller]")]
public class Characters(
    ICreateCharacter createCharacter, 
    IGetCharacter getCharacter,
    IUpdateCharacter updateCharacter,
    IDeleteCharacter deleteCharacter,
    IListCharacters listCharacters) : ControllerBase
{
    [HttpPost]
    [CharacterRequestValidator(RequireIdentifier = false, EnsureRouteMatches = false)]
    public async Task<IActionResult> Create([FromBody] CharacterRequest request)
    {
        try
        {
            var input = new CreateCharacterInput(request.Name);
            var characterId = await createCharacter.Execute(input);
            var response = new CharacterResponse(characterId.Value.ToString(), request.Name);
            return Ok(response);
        }
        catch
        {
            return Problem();
        }
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var characters = await listCharacters.Execute();
        var response = new { items = CharacterContractMapper.ToListItems(characters) };
        return Ok(response);
    }

    [HttpGet("{characterId:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid characterId)
    {
        var id = new CharacterId(characterId);
        var character = await getCharacter.Execute(id);

        if (character is null)
            return new NotFoundResult();

        return Ok(CharacterContractMapper.ToResponse(character));
    }

    [HttpPut("{characterId:guid}")]
    [CharacterRequestValidator(RequireIdentifier = true, EnsureRouteMatches = true)]
    public async Task<IActionResult> Update([FromRoute] Guid characterId, [FromBody] CharacterRequest request)
    {
        try
        {
            var input = new UpdateCharacterInput(characterId, request.Id, request.Name);
            var character = await updateCharacter.Execute(input);
            
            if (character is null)
                return NotFound();
            
            return Ok(CharacterContractMapper.ToResponse(character));
        }
        catch (ArgumentException ex)
        {
            if (string.Equals(ex.ParamName, nameof(UpdateCharacterInput.PayloadId), StringComparison.Ordinal))
            {
                return ValidationProblemFactory.InvalidIdentifier(ex.Message).ToResult();
            }

            return ValidationProblemFactory.InvalidPayload(ex.Message).ToResult();
        }
        catch
        {
            return Problem();
        }
    }

    [HttpDelete("{characterId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid characterId)
    {
        try
        {
            var id = new CharacterId(characterId);
            await deleteCharacter.Execute(id);
            
            return Ok(new { message = "Character deleted successfully", characterId = id.Value.ToString() });
        }
        catch
        {
            return Problem();
        }
    }
}