# Quickstart: Update and Delete Character API

**Feature**: Update and Delete Character API  
**Date**: October 31, 2025  
**Audience**: Developers implementing this feature

## Overview

This feature adds UPDATE and DELETE operations to the Character API, completing the CRUD functionality. It follows the existing IDD architecture pattern with Application layer use cases, Domain services, and Repository interfaces.

## Prerequisites

- Existing Character API with Create and Get operations
- MongoDB running locally or configured connection string
- .NET 8.0 SDK
- Understanding of IDD architecture layers

## Architecture Overview

```text
API Layer (Characters.cs)
    ↓ PUT /api/characters/{id}
    ↓ DELETE /api/characters/{id}
Application Layer (UpdateCharacter, DeleteCharacter use cases)
    ↓
Domain Layer (CharacterService with Update/Delete methods)
    ↓
Repository Interface (ICharacterRepository with Update/Delete)
    ↓
Infrastructure Layer (MongoDbCharacterRepository implementation)
    ↓
MongoDB Database
```

## Implementation Checklist

### Phase 1: Domain Layer (Bottom-Up)

- [ ] **1.1** Add `UpdateCharacterCommand` record to Domain project
- [ ] **1.2** Extend `ICharacterRepository` interface with `Update` and `Delete` methods
- [ ] **1.3** Extend `ICharacterService` interface with `Update` and `Delete` methods
- [ ] **1.4** Implement `Update` and `Delete` in `CharacterService`
- [ ] **1.5** Write unit tests for `CharacterService` new methods

### Phase 2: Infrastructure Layer

- [ ] **2.1** Implement `Update` method in `MongoDbCharacterRepository` using `ReplaceOneAsync`
- [ ] **2.2** Implement `Delete` method in `MongoDbCharacterRepository` using `DeleteOneAsync`
- [ ] **2.3** Write unit tests for repository methods (mock MongoDB)

### Phase 3: Application Layer

- [ ] **3.1** Create `IUpdateCharacter` interface
- [ ] **3.2** Create `UpdateCharacter` use case implementation
- [ ] **3.3** Create `IDeleteCharacter` interface
- [ ] **3.4** Create `DeleteCharacter` use case implementation
- [ ] **3.5** Write unit tests for both use cases (mock `ICharacterService`)

### Phase 4: API Layer

- [ ] **4.1** Add PUT endpoint to `Characters.cs` controller
- [ ] **4.2** Add DELETE endpoint to `Characters.cs` controller
- [ ] **4.3** Register new use cases in `Program.cs` DI container
- [ ] **4.4** Write unit tests for new endpoints (mock use cases)

### Phase 5: Integration Testing

- [ ] **5.1** Write acceptance tests for Update scenarios (Testcontainers)
- [ ] **5.2** Write acceptance tests for Delete scenarios (Testcontainers)
- [ ] **5.3** Verify all edge cases from spec

## Key Implementation Patterns

### Pattern 1: Immutable Character Updates

```csharp
// Domain/CharacterService.cs
public async Task<Character?> Update(UpdateCharacterCommand command)
{
    // Validate business rules
    if (string.IsNullOrWhiteSpace(command.Name))
        throw new ArgumentException("Character name cannot be empty", nameof(command.Name));
    
    // Create new instance (immutable)
    var updatedCharacter = new Character(command.Id, command.Name);
    
    // Persist and return
    var wasUpdated = await repository.Update(updatedCharacter);
    return wasUpdated ? updatedCharacter : null;
}
```

### Pattern 2: Idempotent Delete

```csharp
// Domain/CharacterService.cs
public async Task<bool> Delete(CharacterId characterId)
{
    // Repository returns true if deleted, false if already deleted
    // Service just passes through - idempotency handled at repository level
    return await repository.Delete(characterId);
}
```

### Pattern 3: MongoDB ReplaceOne for Update

```csharp
// Infrastructure/MongoDbCharacterRepository.cs
public async Task<bool> Update(Character character)
{
    var filter = Builders<Character>.Filter.Eq(c => c.Id, character.Id);
    var result = await Characters.ReplaceOneAsync(filter, character);
    
    // MatchedCount > 0 means document was found (even if unchanged)
    return result.MatchedCount > 0;
}
```

### Pattern 4: MongoDB DeleteOne for Delete

```csharp
// Infrastructure/MongoDbCharacterRepository.cs
public async Task<bool> Delete(CharacterId id)
{
    var filter = Builders<Character>.Filter.Eq(c => c.Id, id);
    var result = await Characters.DeleteOneAsync(filter);
    
    // DeletedCount > 0 means document was deleted
    // 0 means already deleted (idempotent)
    return result.DeletedCount > 0;
}
```

### Pattern 5: API Error Handling

```csharp
// API/Characters.cs
[HttpPut("{characterId}")]
public async Task<IActionResult> Update([FromRoute] string characterId, [FromBody] UpdateCharacterRequest request)
{
    try
    {
        // Validate GUID format
        if (!Guid.TryParse(characterId, out var guid))
            return BadRequest(new { error = "Invalid character ID format", statusCode = 400 });
        
        var id = new CharacterId(guid);
        var command = new UpdateCharacterCommand(id, request.Name);
        
        var character = await updateCharacter.Execute(command);
        
        // Null means not found
        if (character is null)
            return NotFound(new { error = "Character not found", statusCode = 404 });
        
        return Ok(JsonSerializer.Serialize(character));
    }
    catch (ArgumentException ex)
    {
        // Business rule violations (e.g., empty name)
        return BadRequest(new { error = ex.Message, statusCode = 400 });
    }
    catch
    {
        return Problem();
    }
}
```

## Testing Strategy

### Unit Tests (NUnit + FakeItEasy)

**Test Class Naming**: `<ClassUnderTest>Should`

**Test Method Naming**: `Capital_snake_case` describing outcome

**Examples**:

```csharp
// Tests.Unit/Domain/CharacterServiceShould.cs
[Test]
public async Task Return_null_when_character_not_found_for_update()
{
    // Arrange
    var mockRepo = A.Fake<ICharacterRepository>();
    A.CallTo(() => mockRepo.Update(A<Character>._)).Returns(false);
    var service = new CharacterService(mockRepo);
    
    // Act
    var result = await service.Update(new UpdateCharacterCommand(someId, "NewName"));
    
    // Assert
    result.Should().BeNull();
}

[Test]
public async Task Throw_exception_when_updating_with_empty_name()
{
    // Arrange
    var mockRepo = A.Fake<ICharacterRepository>();
    var service = new CharacterService(mockRepo);
    
    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() => 
        service.Update(new UpdateCharacterCommand(someId, "")));
}
```

### Integration Tests (Testcontainers)

```csharp
// Tests.Integration/Acceptance/UpdateCharacterTests.cs
[Test]
public async Task Update_character_name_successfully()
{
    // Arrange: Create character first
    var createResponse = await _client.PostAsJsonAsync("/api/characters", 
        new { name = "Aragorn" });
    var characterId = await createResponse.Content.ReadFromJsonAsync<CharacterId>();
    
    // Act: Update character
    var updateResponse = await _client.PutAsJsonAsync($"/api/characters/{characterId.Value}", 
        new { name = "Strider" });
    
    // Assert
    Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    var updatedCharacter = await updateResponse.Content.ReadFromJsonAsync<Character>();
    Assert.That(updatedCharacter.Name, Is.EqualTo("Strider"));
    
    // Verify persistence
    var getResponse = await _client.GetAsync($"/api/characters/{characterId.Value}");
    var retrievedCharacter = await getResponse.Content.ReadFromJsonAsync<Character>();
    Assert.That(retrievedCharacter.Name, Is.EqualTo("Strider"));
}
```

## API Usage Examples

### Update Character

**Request**:

```http
PUT /api/characters/3fa85f64-5717-4562-b3fc-2c963f66afa6 HTTP/1.1
Content-Type: application/json

{
  "name": "Strider"
}
```

**Success Response (200 OK)**:

```json
{
  "id": {
    "value": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  },
  "name": "Strider"
}
```

**Not Found Response (404)**:

```json
{
  "error": "Character not found",
  "statusCode": 404
}
```

**Validation Error (400)**:

```json
{
  "error": "Character name cannot be empty",
  "statusCode": 400
}
```

### Delete Character

**Request**:

```http
DELETE /api/characters/3fa85f64-5717-4562-b3fc-2c963f66afa6 HTTP/1.1
```

**Success Response (200 OK)** - Idempotent:

```json
{
  "message": "Character deleted successfully",
  "characterId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Invalid ID Format (400)**:

```json
{
  "error": "Invalid character ID format",
  "statusCode": 400
}
```

## Local Development Setup

### 1. Ensure MongoDB Running

```powershell
# If using Docker Compose
docker-compose up -d

# Verify MongoDB is accessible
docker ps | Select-String mongo
```

### 2. Build Solution

```powershell
dotnet build src/DrawSteel.sln
```

### 3. Run Tests

```powershell
# Unit tests
dotnet test src/Tests.Unit/Tests.Unit.csproj

# Integration tests
dotnet test src/Tests.Integration/Tests.Integration.csproj
```

### 4. Run API Locally

```powershell
cd src/API
dotnet run
```

### 5. Test Endpoints

```powershell
# Create character
$createResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/characters" `
    -Method POST `
    -ContentType "application/json" `
    -Body '{"name":"Aragorn"}'

$characterId = $createResponse.value

# Update character
Invoke-RestMethod -Uri "http://localhost:5000/api/characters/$characterId" `
    -Method PUT `
    -ContentType "application/json" `
    -Body '{"name":"Strider"}'

# Delete character
Invoke-RestMethod -Uri "http://localhost:5000/api/characters/$characterId" `
    -Method DELETE
```

## Common Pitfalls

### ❌ Don't: Mutate Character Entity

```csharp
// WRONG - Character is immutable
character.Name = "NewName";  // Won't compile
```

### ✅ Do: Create New Instance

```csharp
// CORRECT - Create new instance
var updatedCharacter = new Character(character.Id, "NewName");
```

### ❌ Don't: Return 404 for Already Deleted

```csharp
// WRONG - Breaks idempotency
if (!wasDeleted)
    return NotFound();
```

### ✅ Do: Return Success Regardless

```csharp
// CORRECT - Idempotent delete
var wasDeleted = await deleteCharacter.Execute(id);
return Ok(new { message = "Character deleted successfully", characterId = id.Value });
```

### ❌ Don't: Use UpdateOne with $set

```csharp
// WRONG - Partial updates not needed
var update = Builders<Character>.Update.Set(c => c.Name, newName);
await Characters.UpdateOneAsync(filter, update);
```

### ✅ Do: Use ReplaceOneAsync

```csharp
// CORRECT - Full document replacement
await Characters.ReplaceOneAsync(filter, updatedCharacter);
```

## Next Steps

1. Review [data-model.md](data-model.md) for detailed entity definitions
2. Review [contracts/character-api.openapi.yaml](contracts/character-api.openapi.yaml) for API contract
3. Implement following the checklist above (bottom-up: Domain → Infrastructure → Application → API)
4. Write tests first (TDD approach per constitution)
5. Run integration tests with Testcontainers
6. Create PR for review

## References

- [Feature Specification](spec.md) - Requirements and acceptance criteria
- [Implementation Plan](plan.md) - Technical approach and architecture decisions
- [Research](research.md) - Technology choices and patterns
- [MongoDB .NET Driver Docs](https://mongodb.github.io/mongo-csharp-driver/)
- [ASP.NET Core Web API Docs](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
