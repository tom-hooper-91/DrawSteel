using Application;
using FakeItEasy;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using CreateCharacter = API.CreateCharacter;

namespace Tests.Unit.API;

[TestFixture]
public class CreateCharacterShould
{
    [Test]
    public void Call_CreateCharacter_action()
    {
        var createCharacterAction = A.Fake<ICreateCharacter>();
        var createCharacter = new CreateCharacter(createCharacterAction);
        
        createCharacter.Run(A.Fake<HttpRequest>());
        
        A.CallTo(() => createCharacterAction.Execute()).MustHaveHappened();
    }
}