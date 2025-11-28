# Research: Update and Delete Character API

**Feature**: Update and Delete Character API  
**Date**: October 31, 2025  
**Purpose**: Research best practices and design patterns for implementing update and delete operations in the existing IDD architecture

## Research Tasks

All technical context was clear from existing codebase analysis. No NEEDS CLARIFICATION items identified.

## MongoDB Update and Delete Best Practices

### Decision: Use MongoDB ReplaceOne for Updates and DeleteOne for Deletes

**Rationale**: 
- **ReplaceOne**: Matches existing pattern where Character is an immutable record. Replacing entire document ensures consistency and simplifies concurrency handling with last-write-wins semantics.
- **DeleteOne**: Standard MongoDB deletion method that removes document by ID filter. Idempotent operation (returns 0 if already deleted).

**Best Practices Applied**:
- Use strongly-typed filters with CharacterId for type safety
- Return operation results to distinguish "not found" from "deleted"
- DeleteResult.DeletedCount indicates whether document existed
- ReplaceOneResult.MatchedCount indicates whether document existed

**Alternatives Considered**:
- **UpdateOne with $set**: More appropriate for partial updates. Rejected because Character has only one mutable field (Name) and spec assumes full replacement.
- **FindOneAndReplace**: Returns the replaced document. Rejected because existing pattern returns ID, not full entity.
- **Soft Delete (IsDeleted flag)**: Rejected per spec assumption: "Soft deletes are not required; deletion is permanent"

**References**:
- [MongoDB .NET Driver Documentation - Replace](https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/writing/)
- [MongoDB .NET Driver Documentation - Delete](https://mongodb.github.io/mongo-csharp-driver/2.19/reference/driver/crud/writing/)

## ASP.NET Core REST API Patterns for Update and Delete

### Decision: Use PUT for Update and DELETE for Delete with Standard HTTP Status Codes

**Rationale**:
- **PUT /api/characters/{id}**: RESTful convention for full resource replacement. Matches existing POST/GET pattern.
- **DELETE /api/characters/{id}**: RESTful convention for resource deletion.
- **Status Codes**:
  - 200 OK: Successful update/delete
  - 404 Not Found: Character doesn't exist
  - 400 Bad Request: Invalid input (empty name, invalid GUID)
  - 500 Internal Server Error: Unexpected errors

**Best Practices Applied**:
- Route parameters for resource identification: `[HttpPut("{characterId}")]`
- Request body validation using FromBody attribute
- Consistent error response format across all operations
- Return updated character on success (PUT) or confirmation message (DELETE)

**Alternatives Considered**:
- **PATCH**: For partial updates. Rejected because spec requires full replacement and Character has minimal fields.
- **204 No Content**: Common for DELETE. Rejected to maintain consistency with existing pattern of returning JSON responses.
- **POST for updates**: Non-RESTful. Rejected in favor of standard HTTP verbs.

**References**:
- [Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines/blob/vNext/Guidelines.md)
- [ASP.NET Core Web API Documentation](https://learn.microsoft.com/en-us/aspnet/core/web-api/)

## Error Handling and Validation Patterns

### Decision: Domain-Level Validation with Null Return for Not Found

**Rationale**:
- Maintains existing pattern where CharacterService returns null for missing characters
- API layer translates null to 404 Not Found
- Domain validation prevents invalid states (empty names, invalid GUIDs)
- Repository returns operation results to distinguish "not found" from "operation failed"

**Best Practices Applied**:
- Validate GUID format before passing to domain layer (API responsibility)
- Validate business rules in domain layer (non-empty name)
- Return Result<T> or status indicators from repository for clear success/failure semantics
- Consistent error response format

**Alternatives Considered**:
- **Exceptions for Not Found**: Creates expensive control flow. Rejected in favor of null returns (existing pattern).
- **Result<T> Pattern Throughout**: More explicit but requires refactoring existing code. Deferred as out of scope.
- **FluentValidation**: Adds dependency for minimal validation needs. Rejected per "Minimal Dependencies" principle.

## Idempotency Considerations

### Decision: DELETE Operations Return Success Even if Character Already Deleted

**Rationale**:
- Per spec FR-011: "System MUST handle idempotent delete operations"
- Improves reliability in distributed systems and retry scenarios
- Prevents clients from needing to handle "already deleted" as error condition
- MongoDB DeleteOne naturally returns 0 DeletedCount for missing documents

**Implementation**:
- Check DeleteResult.DeletedCount in repository
- Return success indicator regardless of count (idempotent)
- Service layer can optionally return metadata about operation result
- API returns 200 OK even if character was already deleted

**Alternatives Considered**:
- **404 for Already Deleted**: More precise but breaks idempotency. Rejected.
- **204 No Content**: Would work but inconsistent with existing response patterns. Rejected for consistency.

## Concurrency and Race Conditions

### Decision: Last-Write-Wins with No Optimistic Locking

**Rationale**:
- Per spec assumption: "Concurrent operations follow last-write-wins pattern"
- Minimal complexity for current scale (100 concurrent requests)
- MongoDB atomic operations at document level prevent partial updates
- No evidence of high contention scenarios in character update use case

**Best Practices Applied**:
- All operations atomic at MongoDB document level
- ReplaceOne is atomic - no partial updates possible
- DeleteOne is atomic - no partial deletes possible

**Alternatives Considered**:
- **Optimistic Locking (ETags/Version Field)**: More robust but adds complexity. Rejected per spec assumptions and current scale requirements.
- **Pessimistic Locking**: Not supported by MongoDB document model. Not applicable.
- **CQRS with Event Sourcing**: Over-engineering for current requirements. Rejected per "Simple Design" principle.

**Future Considerations**:
If concurrent update conflicts become an issue, consider adding:
- Version field to Character entity
- ETag support in API layer
- 409 Conflict status code for version mismatches

## Command Pattern for Update

### Decision: Create UpdateCharacterCommand Similar to CreateCharacterCommand

**Rationale**:
- Maintains consistency with existing create operation
- Encapsulates update request data
- Separates API concerns from domain model
- Enables validation at multiple layers

**Structure**:
```csharp
public record UpdateCharacterCommand(CharacterId Id, string Name);
```

**Alternatives Considered**:
- **Use Character Entity Directly**: Couples API to domain model. Rejected per separation of concerns.
- **Separate Command Per Field**: Over-engineering for single field update. Rejected.

## Testing Strategy

### Decision: Three-Layer Testing Approach

**Unit Tests** (NUnit + FakeItEasy):
- Application layer: Mock CharacterService, verify use case orchestration
- Domain layer: Mock ICharacterRepository, verify business logic
- API layer: Mock application use cases, verify HTTP contract

**Integration Tests** (Testcontainers):
- Full stack acceptance tests using MongoDB container
- Test all acceptance scenarios from spec
- Verify end-to-end flow including database persistence

**Test Naming Convention** (per Constitution):
- Class: `UpdateCharacterShould`, `DeleteCharacterShould`
- Methods: Capital_snake_case describing outcome
  - Example: `Return_not_found_when_character_does_not_exist`
  - Example: `Update_character_name_successfully`

**Alternatives Considered**:
- **E2E Tests**: Not needed - integration tests with Testcontainers provide sufficient coverage. Rejected per Test Pyramid principle.
- **Separate Acceptance Test Project**: Existing Tests.Integration project is sufficient. Rejected to avoid additional project overhead.

## Summary

All research complete with clear decisions:

1. ✅ **MongoDB Operations**: ReplaceOne for updates, DeleteOne for deletes
2. ✅ **API Design**: PUT and DELETE with standard REST patterns and status codes
3. ✅ **Error Handling**: Domain-level validation with null returns for not found
4. ✅ **Idempotency**: DELETE returns success even if already deleted
5. ✅ **Concurrency**: Last-write-wins, no optimistic locking required
6. ✅ **Commands**: UpdateCharacterCommand mirrors CreateCharacterCommand
7. ✅ **Testing**: Three-layer approach with unit, integration, no E2E

**No NEEDS CLARIFICATION items remain**. Ready to proceed to Phase 1: Design & Contracts.
