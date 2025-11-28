# Data Model: Update and Delete Character API

**Feature**: Update and Delete Character API  
**Date**: October 31, 2025  
**Purpose**: Define data structures and domain entities for update and delete operations

## Existing Entities (No Changes Required)

### Character

**Type**: Domain Entity (immutable record)

**Definition**:

```csharp
namespace Domain;

public record Character(CharacterId Id, string Name);
```

**Purpose**: Represents a game character. Immutable by design - updates create new instances.

**Validation Rules**:

- `Id`: Must be valid CharacterId (Guid-based)
- `Name`: Must not be null or empty (enforced at domain service level)

**State**: No state transitions - immutable entity

---

### CharacterId

**Type**: Value Object (immutable record)

**Definition**:

```csharp
namespace Domain;

public record CharacterId(Guid Value);
```

**Purpose**: Strongly-typed identifier for Character entities. Prevents primitive obsession.

**Validation Rules**:

- `Value`: Must be valid Guid (enforced by type system)

---

## New Commands

### UpdateCharacterCommand

**Type**: Application Layer Command

**Definition**:

```csharp
namespace Domain;

public record UpdateCharacterCommand(CharacterId Id, string Name);
```

**Purpose**: Encapsulates update request data. Separates API contract from domain model.

**Validation Rules**:

- `Id`: Must be valid CharacterId
- `Name`: Must not be null or empty (validated at domain service level)

**Usage**: Passed from API layer to Application layer use case

---

## Domain Service Changes

### ICharacterService (Interface Extension)

**Modified Methods**:

```csharp
namespace Domain;

public interface ICharacterService
{
    Task<CharacterId> Create(CreateCharacterCommand character);
    Task<Character?> Get(CharacterId characterId);
    Task<Character?> Update(UpdateCharacterCommand command);  // NEW
    Task<bool> Delete(CharacterId characterId);               // NEW
}
```

**New Methods**:

- `Update`: Returns updated Character or null if not found
- `Delete`: Returns true if deleted, false if already deleted (idempotent)

---

### CharacterService (Implementation Changes)

**New Method: Update**

```csharp
public async Task<Character?> Update(UpdateCharacterCommand command)
{
    // Validate name is not empty
    if (string.IsNullOrWhiteSpace(command.Name))
        throw new ArgumentException("Character name cannot be empty", nameof(command.Name));
    
    // Create updated character instance (immutable)
    var updatedCharacter = new Character(command.Id, command.Name);
    
    // Attempt update via repository
    var wasUpdated = await repository.Update(updatedCharacter);
    
    // Return character if updated, null if not found
    return wasUpdated ? updatedCharacter : null;
}
```

**New Method: Delete**

```csharp
public async Task<bool> Delete(CharacterId characterId)
{
    return await repository.Delete(characterId);
}
```

---

## Repository Interface Changes

### ICharacterRepository (Interface Extension)

**Modified Definition**:

```csharp
namespace Domain.Repositories;

public interface ICharacterRepository
{
    Task<CharacterId> Add(Character character);
    Task<Character?> Get(CharacterId id);
    Task<bool> Update(Character character);        // NEW
    Task<bool> Delete(CharacterId id);             // NEW
}
```

**New Methods**:

- `Update`: Returns true if character was found and updated, false if not found
- `Delete`: Returns true if character was found and deleted, false if not found (idempotent)

---

## MongoDB Repository Implementation

### MongoDbCharacterRepository (Implementation Changes)

**New Method: Update**

```csharp
public async Task<bool> Update(Character character)
{
    var filter = Builders<Character>.Filter.Eq(c => c.Id, character.Id);
    var result = await Characters.ReplaceOneAsync(filter, character);
    return result.MatchedCount > 0;
}
```

**MongoDB Operation**: `ReplaceOneAsync` - replaces entire document atomically

**Return Logic**: `MatchedCount > 0` indicates document was found (even if identical)

**New Method: Delete**

```csharp
public async Task<bool> Delete(CharacterId id)
{
    var filter = Builders<Character>.Filter.Eq(c => c.Id, id);
    var result = await Characters.DeleteOneAsync(filter);
    return result.DeletedCount > 0;
}
```

**MongoDB Operation**: `DeleteOneAsync` - deletes document atomically

**Return Logic**: `DeletedCount > 0` indicates document was deleted (false if already deleted - idempotent)

---

## Application Layer Use Cases

### IUpdateCharacter Interface

**Definition**:

```csharp
namespace Application;

public interface IUpdateCharacter
{
    Task<Character?> Execute(UpdateCharacterCommand command);
}
```

**Purpose**: Application layer contract for update use case

---

### UpdateCharacter Implementation

**Definition**:

```csharp
namespace Application;

public class UpdateCharacter(ICharacterService characterService) : IUpdateCharacter
{
    public async Task<Character?> Execute(UpdateCharacterCommand command)
    {
        return await characterService.Update(command);
    }
}
```

**Purpose**: Orchestrates update operation, delegates to domain service

---

### IDeleteCharacter Interface

**Definition**:

```csharp
namespace Application;

public interface IDeleteCharacter
{
    Task<bool> Execute(CharacterId characterId);
}
```

**Purpose**: Application layer contract for delete use case

---

### DeleteCharacter Implementation

**Definition**:

```csharp
namespace Application;

public class DeleteCharacter(ICharacterService characterService) : IDeleteCharacter
{
    public async Task<bool> Execute(CharacterId characterId)
    {
        return await characterService.Delete(characterId);
    }
}
```

**Purpose**: Orchestrates delete operation, delegates to domain service

---

## Entity Relationships

```text
API Layer (Characters.cs)
    ↓ calls
Application Layer (UpdateCharacter, DeleteCharacter)
    ↓ calls
Domain Layer (CharacterService)
    ↓ calls
Repository Interface (ICharacterRepository)
    ↓ implemented by
Infrastructure Layer (MongoDbCharacterRepository)
    ↓ persists to
MongoDB Database
```

**Key Relationships**:

- `UpdateCharacterCommand` → passed from API to Application
- `Character` → returned from Application to API
- `CharacterId` → used across all layers for identification
- `ICharacterService` → abstraction between Application and Domain
- `ICharacterRepository` → abstraction between Domain and Infrastructure

---

## Data Validation Summary

| Layer | Validation Responsibility |
|-------|--------------------------|
| API Layer | GUID format validation, request structure |
| Application Layer | None - delegates to domain |
| Domain Layer | Business rules (non-empty name) |
| Repository Layer | Data persistence concerns only |

---

## State Transitions

### Character Update Flow

```text
1. Client sends PUT /api/characters/{id} with JSON body
2. API validates GUID format and deserializes UpdateCharacterCommand
3. Application use case receives command
4. Domain service validates name is not empty
5. Domain service creates new Character instance (immutable)
6. Repository replaces MongoDB document
7. Repository returns true/false based on MatchedCount
8. Domain service returns Character or null
9. Application returns Character or null
10. API returns 200 OK with Character or 404 Not Found
```

### Character Delete Flow

```text
1. Client sends DELETE /api/characters/{id}
2. API validates GUID format and creates CharacterId
3. Application use case receives CharacterId
4. Domain service calls repository delete
5. Repository deletes MongoDB document
6. Repository returns true/false based on DeletedCount
7. Domain service returns true/false
8. Application returns true/false
9. API returns 200 OK (idempotent - even if already deleted)
```

---

## MongoDB Document Structure

**No changes to document structure**. Character document remains:

```json
{
  "_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "Id": {
    "Value": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  },
  "Name": "Aragorn"
}
```

**Index Considerations**: Existing index on `Id.Value` sufficient for update/delete operations

---

## Summary

**New Entities**: 2

- `UpdateCharacterCommand` (Application command)
- `IUpdateCharacter`, `UpdateCharacter` (Application use cases)
- `IDeleteCharacter`, `DeleteCharacter` (Application use cases)

**Modified Interfaces**: 2

- `ICharacterService` (added Update and Delete methods)
- `ICharacterRepository` (added Update and Delete methods)

**Modified Implementations**: 2

- `CharacterService` (implemented Update and Delete)
- `MongoDbCharacterRepository` (implemented Update and Delete)

**Domain Model Stability**: Character and CharacterId remain unchanged - immutable by design
