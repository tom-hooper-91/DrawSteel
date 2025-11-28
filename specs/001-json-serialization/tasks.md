---

description: "Task list for Consistent Character JSON Contract"
---

# Tasks: Consistent Character JSON Contract

**Input**: Design documents from `/specs/001-json-serialization/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: NUnit for unit coverage, FakeItEasy for isolation, Testcontainers-backed integration tests targeting the Character API contract.

## Format Reminder

`- [ ] T### [P?] [Story?] Description with file path`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Align documentation and tooling so every contributor can run serialization work immediately.

- [X] T001 Document serialization prerequisites and run commands in `specs/001-json-serialization/quickstart.md`
- [X] T002 [P] Add feature overview + contract links to `README.md`
- [X] T003 [P] Seed flattened-id request/response samples for manual smoke tests in `src/API/API.http`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish shared infrastructure (fixtures, diagnostics, serializer configuration) required by all user stories.

- [X] T004 Create shared API + Mongo Testcontainers harness in `src/Tests.Integration/Acceptance/CharacterApiFixture.cs`
- [X] T005 [P] Add reusable JSON contract assertion helpers in `src/Tests.Integration/Acceptance/Assertions/JsonContractAssertions.cs`
- [X] T006 [P] Introduce consistent `ValidationProblemDetails` factory + extensions in `src/API/Diagnostics/ValidationProblemFactory.cs`
- [X] T007 Wire serializer options and the problem factory into the HTTP pipeline in `src/API/Program.cs`

**Checkpoint**: Integration harness, validation infrastructure, and serializer plumbing are ready for story work.

---

## Phase 3: User Story 1 – Consistent Identifier Output (Priority: P1) 🎯 MVP

**Goal**: Every Character API response (create/get/list/update/delete) exposes a single flattened `id` field.

**Independent Test**: Call any Character endpoint and inspect the JSON; the payload must contain `"id":"<guid>"` with no nested `value` keys.

### Tests for User Story 1 (write first, ensure they fail)

- [X] T008 [P] [US1] Update response-shape expectations in `src/Tests.Unit/API/CharactersShould.cs` so assertions fail when `id.value` appears
- [X] T009 [P] [US1] Update `src/Tests.Unit/Application/GetCharacterShould.cs` (and related test doubles) to expect flattened response DTOs
- [X] T010 [P] [US1] Add GET/list/create contract coverage to `src/Tests.Integration/Acceptance/CharacterSerializationTests.cs` that snapshots scalar `id`

### Implementation for User Story 1

- [X] T011 [P] [US1] Add `CharacterResponse` + `CharacterListItem` DTOs exposing `string id` in `src/API/Contracts/CharacterResponse.cs`
- [X] T012 [P] [US1] Implement `CharacterContractMapper` to project `Domain.Character` → DTOs in `src/API/Contracts/CharacterContractMapper.cs`
- [X] T013 [US1] Introduce `CharacterDto` outputs and update use cases in `src/Application/IGetCharacter.cs`, `src/Application/GetCharacter.cs`, `src/Application/IUpdateCharacter.cs`, `src/Application/UpdateCharacter.cs`
- [X] T014 [US1] Rewrite `src/API/Characters.cs` endpoints to rely on the mapper and return strongly typed DTOs across every response
- [X] T015 [US1] Refresh response schemas/examples in `specs/001-json-serialization/contracts/character-json-contract.openapi.yaml` to reflect scalar `id`
- [X] T016 [US1] Run serialization suite via `src/Tests.Integration/Tests.Integration.csproj` to prove P1 contract stability

**Checkpoint**: All outbound payloads now share the same `id` field; integration tests document the contract.

---

## Phase 4: User Story 2 – Predictable Request Payloads (Priority: P2)

**Goal**: Create/update endpoints accept (and validate) flattened `id` payloads while rejecting legacy `id.value` shapes.

**Independent Test**: POST/PUT a JSON body containing `id` as a top-level property—API accepts it or returns a clear validation message; legacy payloads are rejected with actionable errors.

### Tests for User Story 2 (write first, ensure they fail)

- [ ] T017 [P] [US2] Create request-validation unit coverage in `src/Tests.Unit/API/CharacterRequestValidatorShould.cs` for scalar `id` and legacy rejection
- [ ] T018 [P] [US2] Update `src/Tests.Unit/Application/UpdateCharacterShould.cs` to enforce body/route `id` alignment and DTO conversion rules
- [ ] T019 [P] [US2] Extend POST/PUT coverage in `src/Tests.Integration/Acceptance/CharacterRequestContractTests.cs` for accepted vs rejected payloads

### Implementation for User Story 2

- [ ] T020 [P] [US2] Define `CharacterRequest` DTO with validation attributes in `src/API/Requests/CharacterRequest.cs`
- [ ] T021 [P] [US2] Add `CharacterRequestValidator` (model binder/filter) that rejects nested `id.value` payloads in `src/API/Validation/CharacterRequestValidator.cs`
- [ ] T022 [US2] Update `src/API/Characters.cs` POST/PUT handlers to bind `CharacterRequest`, enforce route/body `id` parity, and emit structured validation errors
- [ ] T023 [US2] Translate request DTOs into domain commands inside `src/Application/CreateCharacter.cs` and `src/Application/UpdateCharacter.cs`
- [ ] T024 [US2] Document the accepted request schemas and error responses in `specs/001-json-serialization/contracts/character-json-contract.openapi.yaml`
- [ ] T025 [US2] Add POST/PUT samples (happy path + validation failure) to `src/API/API.http`
- [ ] T026 [US2] Re-run unit + integration suites via `src/Tests.Unit/Tests.Unit.csproj` and `src/Tests.Integration/Tests.Integration.csproj` to validate request handling

**Checkpoint**: Clients can send predictable flattened payloads; legacy shapes produce clear guidance.

---

## Phase 5: User Story 3 – Contract-Focused Integration Tests (Priority: P3)

**Goal**: Guard the HTTP contract with fixture-driven integration tests that fail loudly on drift.

**Independent Test**: Execute the contract test suite; it must diff actual JSON vs fixtures for CRUD and validation flows, failing whenever the schema changes.

### Tests for User Story 3 (write first, ensure they fail)

- [X] T027 [P] [US3] Author canonical CRUD + error fixtures under `specs/001-json-serialization/contracts/fixtures/*.json`
- [X] T028 [P] [US3] Build happy-path snapshot tests in `src/Tests.Integration/Acceptance/CharacterContractTests.cs` comparing API output to fixtures
- [X] T029 [P] [US3] Add validation/error snapshot coverage (invalid GUID, legacy payload) to `src/Tests.Integration/Acceptance/CharacterContractTests.cs`

### Implementation for User Story 3

- [X] T030 [US3] Implement fixture loader/updater utilities in `src/Tests.Integration/Acceptance/Fixtures/ContractFixtureStore.cs`
- [ ] T031 [US3] Tag and include contract tests in CI by updating `src/Tests.Integration/Tests.Integration.csproj`
- [ ] T032 [US3] Document fixture maintenance + contract test workflow in `specs/001-json-serialization/quickstart.md`
- [ ] T033 [US3] Publish a fixture index / consumer guidance in `specs/001-json-serialization/contracts/README.md`

**Checkpoint**: Contract fixtures + tests fail on schema drift and provide clear consumer documentation.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final hardening, documentation, and regression safety nets spanning all stories.

- [ ] T034 [P] Capture a contract-drift checklist (fixture update, OpenAPI sync, verification steps) in `specs/001-json-serialization/checklists/contract-drift.md`
- [ ] T035 [P] Append final serialization/validation decisions + trade-offs to `specs/001-json-serialization/research.md`
- [ ] T036 Run a full solution verification pass via `src/DrawSteel.sln` (dotnet test + formatting) before release

---

## Dependencies & Execution Order

1. **Phase 1 → Phase 2**: Documentation and baseline tooling must be ready before building shared infrastructure.
2. **Phase 2 → User Stories**: Test fixtures, serializer config, and validation plumbing (T004–T007) unblock all story work.
3. **User Stories**: Execute in priority order (US1 → US2 → US3) or in parallel once dependencies are satisfied:
   - US2 depends on US1 data contracts.
   - US3 depends on US1+US2 to stabilize payloads before snapshotting.
4. **Polish** runs after the targeted user stories complete.

---

## Parallel Execution Examples per Story

- **US1**: T008 and T009 can proceed simultaneously (separate test projects), while T011 and T012 can be parallelized because they touch different files.
- **US2**: T017 and T018 can run in parallel, and T020/T021 may proceed concurrently once validators are sketched.
- **US3**: T027 fixture authoring can happen while T028 test scaffolding is written; T030 utility work can start once fixtures exist.

---

## Implementation Strategy

1. **MVP (US1)**: Complete Phases 1–3, run integration tests (T016), and ship the consistent response contract.
2. **Incremental Delivery**: Layer US2 to align request payloads, then US3 to guard the contract. Each story remains independently demoable/testing.
3. **Parallel Teaming**: After Phase 2, one contributor tackles US1 mapping, another handles US2 validators, and a third focuses on US3 fixtures/tests.
4. **Regression Discipline**: Every story ends with suite execution tasks (T016, T026) so failures surface early and feed into contract snapshots.
