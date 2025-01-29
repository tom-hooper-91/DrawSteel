﻿using API;
using Application;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Unit.API;

[TestFixture]
public class CharactersShould
{
    private Characters _createCharacterApi;
    private ICreateCharacter _createCharacterAction;

    [SetUp]
    public void Setup()
    {
        _createCharacterAction = A.Fake<ICreateCharacter>();
        _createCharacterApi = new Characters(_createCharacterAction);
    }
    
    [TestCase("Frodo")]
    [TestCase("Sam")]
    public void Call_CreateCharacter_action_on_post(string characterName)
    {
        var command = new CreateCharacterCommand(characterName);
        var fakeRequest = A.Fake<HttpRequest>();
        fakeRequest.Body = new StringContent(JsonSerializer.Serialize(new { Name = characterName })).ReadAsStream();
        var expectedCharacterId = new CharacterId(Guid.NewGuid());
        A.CallTo(() => _createCharacterAction.Execute(command)).Returns(expectedCharacterId);
        
        var response = _createCharacterApi.Create(fakeRequest) as OkObjectResult;
        var characterId = JsonSerializer.Deserialize<CharacterId>(response!.Value!.ToString()!);

        A.CallTo(() => _createCharacterAction.Execute(command)).MustHaveHappened();
        Assert.That(characterId, Is.EqualTo(expectedCharacterId));
    }
}