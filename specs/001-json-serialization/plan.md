# Implementation Plan: Consistent Character JSON Contract

**Branch**: `001-json-serialization` | **Date**: November 28, 2025 | **Spec**: [specs/001-json-serialization/spec.md](specs/001-json-serialization/spec.md)
**Input**: Feature specification from `/specs/001-json-serialization/spec.md`

## Summary

Fix the Character API's inconsistent identifier serialization by standardizing every request and response to use a single `id` property while dropping legacy `id.value` shapes, then harden integration tests so future contract drift is automatically caught. Implementation centers on domain-level DTO adjustments, serialization configuration within the API project, and expanded HTTP-level Testcontainers coverage plus refreshed documentation fixtures.

## Technical Context

**Language/Version**: C# / .NET 8.0
**Architecture**: Interaction-Driven Design (IDD) with Application layer mediating between Presentation and Domain
**Primary Dependencies**: Microsoft.AspNetCore, MongoDB.Driver, MongoDB.Bson.Serialization, NUnit, FakeItEasy, Testcontainers
**Storage**: MongoDB (local Testcontainers instance; Cosmos DB for Mongo API in production)
**Testing**: NUnit (unit/integration), FakeItEasy for isolation, Testcontainers for API + Mongo contract coverage
**Target Platform**: Azure Container Apps in production; Docker Compose locally
**Presentation Layer**: API (ASP.NET Core Web API); Web UI unaffected
**Performance Goals**: Character endpoints continue to respond within 1 second P95 while enforcing stricter validation
**Constraints**: Maintain backward-compatible GUID semantics without versioning the API; no new runtime dependencies beyond serializers/test utilities
**Scale/Scope**: Covers all Character endpoints (create, list, retrieve, update, delete) supporting dozens of concurrent clients typical for tabletop campaign tooling

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **IDD Architecture**: Application layer contracts will supply flattened DTOs; API never bypasses Application/Domain
- [x] **Testing Standards**: NUnit, FakeItEasy, and Testcontainers already in stack and planned for new unit + integration suites
- [x] **Test Pyramid**: Majority of work remains in unit-level DTO/mapper tests with targeted integration tests for HTTP contract drift
- [x] **Clean Code & SOLID**: Adjustments isolated to serializers, DTOs, and repositories with clear responsibilities
- [x] **Minimal Dependencies**: Reuse existing serializers/framework components—no new libraries introduced
- [x] **IaC Compliance**: No infrastructure changes required; Terraform untouched
- [x] **CI/CD**: Existing GitHub Actions workflows execute updated unit/integration suites automatically

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
# Interaction-Driven Design (IDD) Structure
src/
├── API/                 # ASP.NET Core Web API presentation layer
│   ├── Characters.cs    # Minimal API endpoints exposing Character routes
│   ├── Requests/        # HTTP request DTOs (to align serialization contract)
│   └── Program.cs       # Serializer configuration and endpoint wiring
├── Application/         # Use cases orchestrating domain operations
│   ├── CreateCharacter.cs / UpdateCharacter.cs / GetCharacter.cs / DeleteCharacter.cs
│   ├── ICreateCharacter.cs ... (interfaces for each use case)
│   └── DTO adjustments (if required) to normalize `id`
├── Domain/              # Core entities and services
│   ├── Character.cs / CharacterId.cs
│   ├── CharacterService.cs
│   ├── CreateCharacterCommand.cs / UpdateCharacterCommand.cs
│   └── Repositories/
│       └── ICharacterService.cs & repository contracts (already present)
├── Infrastructure/      # Mongo persistence and setup
│   ├── MongoDbCharacterRepository.cs (ensures stored/retrieved docs use flattened id)
│   └── ConfigureDatabase.cs / DatabaseConstants.cs
└── Web/                 # Blazor UI (unchanged by this feature)
    └── Components/

Tests.Unit/
├── API/                 # Serializer + mapping guard tests
├── Application/         # DTO/use-case regression tests
└── Domain/              # Character entity/test coverage (existing)

Tests.Integration/
└── Acceptance/
    ├── CharacterSerializationTests.cs (new HTTP contract tests)
    └── Existing fixtures expanded for CRUD coverage
```

**Structure Decision**: Feature touches only the API presentation layer plus shared Application/Domain/Infrastructure DTOs. Web/Blazor stays untouched. Endpoints continue to route through `Characters.cs`, ensuring IDD layering while serializers are configured centrally in `Program.cs` and validated via Application DTOs.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| *(none)* | — | — |

## Constitution Re-Check (Post-Design)

- [x] IDD layering preserved—Presentation changes stay within API and map through Application/Domain.
- [x] Testing additions keep the pyramid intact (unit mapper tests + targeted Testcontainers coverage).
- [x] No new dependencies introduced; serialization relies on existing System.Text.Json.
- [x] Clean code guardrails maintained (DTOs isolate formatting so Domain stays pure).
