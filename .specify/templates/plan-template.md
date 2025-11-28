# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for this specific feature. The defaults below reflect the Draw Steel project stack.
-->

**Language/Version**: C# / .NET [specify version if relevant]
**Architecture**: Interaction-Driven Design (IDD) with Application layer mediating between Presentation and Domain
**Primary Dependencies**: [e.g., MongoDB.Driver, Microsoft.AspNetCore, Microsoft.AspNetCore.Components or NEEDS CLARIFICATION]
**Storage**: MongoDB (containerized locally, Azure CosmosDB for MongoDB API in production)
**Testing**: NUnit (unit & integration), FakeItEasy (mocking), Testcontainers (integration dependencies)
**Target Platform**: Azure Container Apps (production), Docker Compose (local development)
**Presentation Layer**: [Specify: API (ASP.NET Core Web API) OR Web (Blazor) OR Both - per feature requirements]
**Performance Goals**: [domain-specific, e.g., <200ms API response time, interactive UI updates or NEEDS CLARIFICATION]
**Constraints**: [domain-specific, e.g., Azure free tier limits, MongoDB document size limits or NEEDS CLARIFICATION]
**Scale/Scope**: [domain-specific, e.g., 100 concurrent users, 10k characters, single-page form or NEEDS CLARIFICATION]

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [ ] **IDD Architecture**: Confirms Application layer mediates between Presentation (API/Web) and Domain
- [ ] **Testing Standards**: NUnit, FakeItEasy, and Testcontainers planned for appropriate test levels
- [ ] **Test Pyramid**: Unit tests dominate, integration tests moderate, E2E minimal (critical paths only)
- [ ] **Clean Code & SOLID**: Design respects single responsibility, dependency inversion, and clear interfaces
- [ ] **Minimal Dependencies**: New dependencies justified and well-maintained
- [ ] **IaC Compliance**: Infrastructure changes use Terraform with azurerm provider (azapi only if required)
- [ ] **CI/CD**: GitHub Actions workflow planned for testing, build, and deployment gates

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
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths specific to this feature. The delivered plan must not include Option labels.
-->

```text
# Interaction-Driven Design (IDD) Structure
src/
├── API/                 # ASP.NET Core Web API presentation layer (if feature uses API)
│   ├── [Feature].cs
│   └── Program.cs
├── Application/         # Use cases and orchestration
│   ├── I[UseCase].cs    # Interface
│   └── [UseCase].cs     # Implementation
├── Domain/              # Business logic and entities
│   ├── [Entity].cs
│   ├── [EntityId].cs
│   ├── [Service].cs
│   └── Repositories/
│       └── I[Name]Repository.cs
├── Infrastructure/      # External concerns (DB, APIs)
│   ├── [Name]Repository.cs
│   └── ConfigureDatabase.cs
└── Web/                 # Blazor presentation layer (if feature uses Web)
    ├── Components/
    │   └── [Feature].razor
    └── Program.cs

Tests.Unit/              # Fast, isolated unit tests (NUnit + FakeItEasy)
├── API/
├── Application/
└── Domain/

Tests.Integration/       # Cross-boundary tests (Testcontainers)
└── Acceptance/
    └── [Feature]Tests.cs
```

**Structure Decision**: [Document which presentation layer (API or Web) is used for this feature and any specific architectural decisions]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
