# Implementation Plan: Update and Delete Character API

**Branch**: `001-character-api` | **Date**: October 31, 2025 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-character-api/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Add update and delete operations to the Character API, extending the existing create and retrieve functionality. This feature enables users to modify character names and permanently remove characters from the system. Implementation follows the existing IDD architecture pattern with Application layer use cases, Domain services, and Repository interfaces, exposed through ASP.NET Core Web API endpoints.

## Technical Context

**Language/Version**: C# / .NET 8.0
**Architecture**: Interaction-Driven Design (IDD) with Application layer mediating between Presentation and Domain
**Primary Dependencies**: MongoDB.Driver, Microsoft.AspNetCore
**Storage**: MongoDB (containerized locally, Azure CosmosDB for MongoDB API in production)
**Testing**: NUnit (unit & integration), FakeItEasy (mocking), Testcontainers (integration dependencies)
**Target Platform**: Azure Container Apps (production), Docker Compose (local development)
**Presentation Layer**: API (ASP.NET Core Web API)
**Performance Goals**: <2 seconds for update/delete operations, <1 second for error responses
**Constraints**: MongoDB document size limits (16MB), Azure Container Apps free tier resource limits
**Scale/Scope**: Support 100 concurrent update/delete requests, maintain consistency with existing CRUD operations

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Initial Check (Pre-Research)**: ✅ PASSED

**Post-Design Re-evaluation**: ✅ PASSED

- [x] **IDD Architecture**: Confirms Application layer mediates between Presentation (API) and Domain - follows existing pattern with UpdateCharacter/DeleteCharacter use cases
- [x] **Testing Standards**: NUnit, FakeItEasy, and Testcontainers planned for appropriate test levels
- [x] **Test Pyramid**: Unit tests dominate (Application + Domain + Repository), integration tests moderate (acceptance scenarios), E2E minimal (none needed - covered by integration tests)
- [x] **Clean Code & SOLID**: Design respects single responsibility (separate use cases), dependency inversion (repository interface), and clear interfaces (IUpdateCharacter, IDeleteCharacter)
- [x] **Minimal Dependencies**: No new dependencies required - uses existing MongoDB.Driver and ASP.NET Core
- [x] **IaC Compliance**: No infrastructure changes required - uses existing MongoDB and API infrastructure
- [x] **CI/CD**: Existing GitHub Actions workflow covers testing, build, and deployment gates

**Status**: ✅ PASSED - No violations, follows existing architecture patterns. Design artifacts confirm compliance.

## Project Structure

### Documentation (this feature)

```text
specs/001-character-api/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
│   └── character-api.openapi.yaml
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
# Interaction-Driven Design (IDD) Structure
src/
├── API/                          # ASP.NET Core Web API presentation layer
│   ├── Characters.cs             # [MODIFIED] Add PUT and DELETE endpoints
│   └── Program.cs                # [MODIFIED] Register new use cases
├── Application/                  # Use cases and orchestration
│   ├── IUpdateCharacter.cs       # [NEW] Update use case interface
│   ├── UpdateCharacter.cs        # [NEW] Update use case implementation
│   ├── IDeleteCharacter.cs       # [NEW] Delete use case interface
│   ├── DeleteCharacter.cs        # [NEW] Delete use case implementation
│   ├── ICreateCharacter.cs       # [EXISTING]
│   ├── CreateCharacter.cs        # [EXISTING]
│   ├── IGetCharacter.cs          # [EXISTING]
│   └── GetCharacter.cs           # [EXISTING]
├── Domain/                       # Business logic and entities
│   ├── Character.cs              # [EXISTING]
│   ├── CharacterId.cs            # [EXISTING]
│   ├── ICharacterService.cs      # [MODIFIED] Add Update and Delete methods
│   ├── CharacterService.cs       # [MODIFIED] Implement Update and Delete
│   ├── CreateCharacterCommand.cs # [EXISTING]
│   ├── UpdateCharacterCommand.cs # [NEW] Command for update operation
│   └── Repositories/
│       └── ICharacterRepository.cs  # [MODIFIED] Add Update and Delete methods
├── Infrastructure/               # External concerns (DB, APIs)
│   ├── MongoDbCharacterRepository.cs # [MODIFIED] Implement Update and Delete
│   └── ConfigureDatabase.cs      # [EXISTING]

Tests.Unit/                       # Fast, isolated unit tests (NUnit + FakeItEasy)
├── API/
│   └── CharactersShould.cs       # [MODIFIED] Add tests for PUT and DELETE endpoints
├── Application/
│   ├── UpdateCharacterShould.cs  # [NEW] Unit tests for UpdateCharacter use case
│   └── DeleteCharacterShould.cs  # [NEW] Unit tests for DeleteCharacter use case
└── Domain/
    └── CharacterServiceShould.cs # [MODIFIED] Add tests for Update and Delete methods

Tests.Integration/                # Cross-boundary tests (Testcontainers)
└── Acceptance/
    ├── UpdateCharacterTests.cs   # [NEW] Integration tests for update scenarios
    └── DeleteCharacterTests.cs   # [NEW] Integration tests for delete scenarios
```

**Structure Decision**: Uses API presentation layer only (no Web/Blazor components). Follows existing IDD pattern with clear layer separation. All new components mirror existing patterns (CreateCharacter → UpdateCharacter, DeleteCharacter).

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

**Status**: No violations - all constitution gates passed. No complexity justifications required.

---

## Planning Phases Completed

### Phase 0: Outline & Research ✅

**Output**: [research.md](research.md)

**Key Decisions**:
- MongoDB ReplaceOne for updates, DeleteOne for deletes
- PUT and DELETE HTTP verbs with RESTful patterns
- Domain-level validation with null returns for not found
- Idempotent delete operations
- Last-write-wins concurrency model
- UpdateCharacterCommand pattern matching CreateCharacterCommand
- Three-layer testing approach (unit/integration/no E2E)

**Status**: All NEEDS CLARIFICATION items resolved. No unknowns remaining.

### Phase 1: Design & Contracts ✅

**Outputs**:
- [data-model.md](data-model.md) - Entity definitions and relationships
- [contracts/character-api.openapi.yaml](contracts/character-api.openapi.yaml) - OpenAPI 3.0 specification
- [quickstart.md](quickstart.md) - Developer implementation guide
- [.github/copilot-instructions.md](../../.github/copilot-instructions.md) - Updated agent context

**Key Artifacts**:
- 2 new Application use cases (IUpdateCharacter, IDeleteCharacter)
- 1 new Domain command (UpdateCharacterCommand)
- 2 new repository methods (Update, Delete)
- 2 new domain service methods (Update, Delete)
- 2 new API endpoints (PUT, DELETE)
- Complete API contract with all status codes and examples

**Constitution Re-check**: ✅ PASSED - Design confirms all gates met

### Phase 2: Task Breakdown (Next Command)

**Status**: NOT STARTED - Use `/speckit.tasks` command to generate tasks.md

**Expected Output**: Prioritized task breakdown by user story with:
- Detailed implementation steps
- Test-first approach (write tests before code)
- Clear acceptance criteria per task
- Dependency ordering

---

## Summary

**Feature**: Update and Delete Character API
**Branch**: `001-character-api`
**Planning Status**: COMPLETE (Phases 0-1)

**Constitution Compliance**: ✅ All gates passed
**New Dependencies**: None - uses existing stack
**Infrastructure Changes**: None - uses existing MongoDB and API infrastructure

**Artifacts Generated**:
1. ✅ research.md - Technology decisions and best practices
2. ✅ data-model.md - Entity definitions and data flow
3. ✅ contracts/character-api.openapi.yaml - API specification
4. ✅ quickstart.md - Implementation guide for developers
5. ✅ Updated agent context for GitHub Copilot

**Ready for**: `/speckit.tasks` command to generate detailed task breakdown

**Implementation Estimate**: 
- 2 new use cases
- 2 new domain methods
- 2 new repository methods
- 2 new API endpoints
- ~15-20 unit tests
- ~5-8 integration tests
- Estimated effort: 1-2 days following TDD approach
