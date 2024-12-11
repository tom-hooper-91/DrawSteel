using API;
using Application;
using FakeItEasy;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;

namespace Tests.Unit.API;

[TestFixture]
public class CreateCharacterShould
{
    [Test]
    public void Call_CreateCharacter_action()
    {
        var createCharacterAction = A.Fake<ICreateCharacter>();
        var createCharacterApi = new Characters(createCharacterAction);
        
        createCharacterApi.Create(A.Fake<HttpRequest>());
        
        A.CallTo(() => createCharacterAction.Execute()).MustHaveHappened();
    }
}