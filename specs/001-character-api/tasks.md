# Tasks: Update and Delete Character API

**Input**: Design documents from `/specs/001-character-api/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Tests follow our testing standards: NUnit for all tests, FakeItEasy for mocking, Testcontainers for integration tests. Tests are written FIRST (TDD) and must fail before implementation.

**Test Naming**: Test classes named `<ClassUnderTest>Should` (e.g., `CharacterServiceShould`, `UpdateCharacterShould`). Test methods use Capital_snake_case describing outcomes (e.g., `Return_updated_character_when_found`, `Return_null_when_character_not_found`). No Arrange-Act-Assert comments - structure should be self-evident.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

Based on Interaction-Driven Design (IDD) architecture:

- **Domain Layer**: `src/Domain/` - Entities, domain services, repository interfaces
- **Application Layer**: `src/Application/` - Use cases, orchestration (I[UseCase] interface + implementation)
- **API Layer**: `src/API/` - ASP.NET Core Web API endpoints
- **Infrastructure Layer**: `src/Infrastructure/` - Repository implementations, external services
- **Unit Tests**: `src/Tests.Unit/[Layer]/` - NUnit tests with FakeItEasy mocks
- **Integration Tests**: `src/Tests.Integration/Acceptance/` - Testcontainers-based integration tests

**Communication Rule**: API MUST communicate with Domain through Application layer only.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Verify existing infrastructure is ready for new operations

- [X] T001 Verify MongoDB is running and accessible locally via Docker Compose
- [X] T002 Verify existing Character entity and CharacterId are available in src/Domain/
- [X] T003 [P] Verify existing ICharacterService and CharacterService are available in src/Domain/
- [X] T004 [P] Verify existing ICharacterRepository interface is available in src/Domain/Repositories/

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Create shared commands and extend core interfaces needed by ALL user stories

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T005 [P] Create UpdateCharacterCommand record in src/Domain/UpdateCharacterCommand.cs
- [X] T006 Extend ICharacterService interface with Update and Delete method signatures in src/Domain/ICharacterService.cs
- [X] T007 Extend ICharacterRepository interface with Update and Delete method signatures in src/Domain/Repositories/ICharacterRepository.cs

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Update Character Name (Priority: P1) 🎯 MVP

**Goal**: Enable users to modify an existing character's name, allowing corrections and updates as their game progresses.

**Independent Test**: Create a character, call PUT endpoint with new name, verify name changed and ID preserved. Subsequent GET should return updated name.

### Tests for User Story 1 ⚠️ **TDD: Write FIRST, Ensure FAIL**

> **MANDATORY**: These tests MUST be written before implementation, MUST fail initially, then pass after implementation

- [X] T008 [P] [US1] Unit test CharacterServiceShould.Update_character_name_successfully using NUnit & FakeItEasy in src/Tests.Unit/Domain/CharacterServiceShould.cs
- [X] T009 [P] [US1] Unit test CharacterServiceShould.Return_null_when_updating_nonexistent_character using NUnit & FakeItEasy in src/Tests.Unit/Domain/CharacterServiceShould.cs
- [X] T010 [P] [US1] Unit test CharacterServiceShould.Throw_exception_when_updating_with_empty_name using NUnit & FakeItEasy in src/Tests.Unit/Domain/CharacterServiceShould.cs
- [X] T011 [P] [US1] Unit test MongoDbCharacterRepository.Update_returns_true_when_character_found (mock MongoDB) in src/Tests.Unit/Infrastructure/MongoDbCharacterRepositoryShould.cs
- [X] T012 [P] [US1] Unit test MongoDbCharacterRepository.Update_returns_false_when_character_not_found (mock MongoDB) in src/Tests.Unit/Infrastructure/MongoDbCharacterRepositoryShould.cs
- [X] T013 [P] [US1] Create IUpdateCharacter interface in src/Application/IUpdateCharacter.cs
- [X] T014 [P] [US1] Unit test UpdateCharacterShould.Delegate_to_character_service using NUnit & FakeItEasy in src/Tests.Unit/Application/UpdateCharacterShould.cs
- [X] T015 [P] [US1] Unit test UpdateCharacterShould.Return_null_when_service_returns_null using NUnit & FakeItEasy in src/Tests.Unit/Application/UpdateCharacterShould.cs
- [X] T016 [P] [US1] Unit test CharactersShould.Return_200_with_updated_character_on_successful_update in src/Tests.Unit/API/CharactersShould.cs
- [X] T017 [P] [US1] Unit test CharactersShould.Return_404_when_updating_nonexistent_character in src/Tests.Unit/API/CharactersShould.cs
- [X] T018 [P] [US1] Unit test CharactersShould.Return_400_when_updating_with_invalid_guid in src/Tests.Unit/API/CharactersShould.cs
- [X] T019 [P] [US1] Integration test UpdateCharacterTests.Update_character_name_successfully using Testcontainers in src/Tests.Integration/Acceptance/UpdateCharacterTests.cs
- [X] T020 [P] [US1] Integration test UpdateCharacterTests.Return_404_when_character_not_found using Testcontainers in src/Tests.Integration/Acceptance/UpdateCharacterTests.cs
- [X] T021 [P] [US1] Integration test UpdateCharacterTests.Persist_update_across_get_requests using Testcontainers in src/Tests.Integration/Acceptance/UpdateCharacterTests.cs

### Implementation for User Story 1

- [X] T022 [US1] Implement Update method in CharacterService in src/Domain/CharacterService.cs (validate name, create new Character instance, call repository)
- [X] T023 [US1] Implement Update method in MongoDbCharacterRepository using ReplaceOneAsync in src/Infrastructure/MongoDbCharacterRepository.cs
- [X] T024 [US1] Create UpdateCharacter use case implementation in src/Application/UpdateCharacter.cs
- [X] T025 [US1] Add PUT endpoint to Characters controller in src/API/Characters.cs (validate GUID, create command, handle null return as 404)
- [X] T026 [US1] Register IUpdateCharacter and UpdateCharacter in DI container in src/API/Program.cs
- [X] T027 [US1] Verify all US1 tests pass (green phase of TDD)
- [X] T028 [US1] Manual test: Create character, update via PUT, verify via GET

**Checkpoint**: At this point, User Story 1 should be fully functional - users can update character names

---

## Phase 4: User Story 2 - Delete Character (Priority: P2)

**Goal**: Enable users to permanently remove characters from the system when no longer needed (e.g., character death, cleanup).

**Independent Test**: Create a character, call DELETE endpoint, verify character no longer retrievable via GET (404 response). Subsequent DELETE should still return success (idempotent).

### Tests for User Story 2 ⚠️ **TDD: Write FIRST, Ensure FAIL**

- [X] T029 [P] [US2] Unit test CharacterServiceShould.Delete_character_successfully using NUnit & FakeItEasy in src/Tests.Unit/Domain/CharacterServiceShould.cs
- [X] T030 [P] [US2] Unit test CharacterServiceShould.Return_false_when_deleting_nonexistent_character using NUnit & FakeItEasy in src/Tests.Unit/Domain/CharacterServiceShould.cs
- [X] T031 [P] [US2] Unit test MongoDbCharacterRepository.Delete_returns_true_when_character_deleted (mock MongoDB) in src/Tests.Unit/Infrastructure/MongoDbCharacterRepositoryShould.cs
- [X] T032 [P] [US2] Unit test MongoDbCharacterRepository.Delete_returns_false_when_already_deleted (mock MongoDB) in src/Tests.Unit/Infrastructure/MongoDbCharacterRepositoryShould.cs
- [X] T033 [P] [US2] Create IDeleteCharacter interface in src/Application/IDeleteCharacter.cs
- [X] T034 [P] [US2] Unit test DeleteCharacterShould.Delegate_to_character_service using NUnit & FakeItEasy in src/Tests.Unit/Application/DeleteCharacterShould.cs
- [X] T035 [P] [US2] Unit test DeleteCharacterShould.Return_false_when_service_returns_false using NUnit & FakeItEasy in src/Tests.Unit/Application/DeleteCharacterShould.cs
- [X] T036 [P] [US2] Unit test CharactersShould.Return_200_on_successful_delete in src/Tests.Unit/API/CharactersShould.cs
- [X] T037 [P] [US2] Unit test CharactersShould.Return_200_on_idempotent_delete in src/Tests.Unit/API/CharactersShould.cs
- [X] T038 [P] [US2] Unit test CharactersShould.Return_400_when_deleting_with_invalid_guid in src/Tests.Unit/API/CharactersShould.cs
- [X] T039 [P] [US2] Integration test DeleteCharacterTests.Delete_character_successfully using Testcontainers in src/Tests.Integration/Acceptance/DeleteCharacterTests.cs
- [X] T040 [P] [US2] Integration test DeleteCharacterTests.Character_not_retrievable_after_delete using Testcontainers in src/Tests.Integration/Acceptance/DeleteCharacterTests.cs
- [X] T041 [P] [US2] Integration test DeleteCharacterTests.Delete_returns_success_when_already_deleted using Testcontainers in src/Tests.Integration/Acceptance/DeleteCharacterTests.cs

### Implementation for User Story 2

- [X] T042 [US2] Implement Delete method in CharacterService in src/Domain/CharacterService.cs (delegate to repository)
- [X] T043 [US2] Implement Delete method in MongoDbCharacterRepository using DeleteOneAsync in src/Infrastructure/MongoDbCharacterRepository.cs
- [X] T044 [US2] Create DeleteCharacter use case implementation in src/Application/DeleteCharacter.cs
- [X] T045 [US2] Add DELETE endpoint to Characters controller in src/API/Characters.cs (validate GUID, return 200 regardless of result for idempotency)
- [X] T046 [US2] Register IDeleteCharacter and DeleteCharacter in DI container in src/API/Program.cs
- [X] T047 [US2] Verify all US2 tests pass (green phase of TDD)
- [X] T048 [US2] Manual test: Create character, delete via DELETE, verify 404 on GET, verify second DELETE returns 200

**Checkpoint**: At this point, User Stories 1 AND 2 should both work - users can update and delete characters

---

## Phase 5: User Story 3 - Safe Update Operations (Priority: P3)

**Goal**: Provide clear feedback when update operations fail (non-existent character, invalid input), improving user experience.

**Independent Test**: Attempt to update non-existent character ID, verify 404 response. Attempt to update with invalid GUID format, verify 400 response. Attempt to update with empty name, verify 400 response.

### Tests for User Story 3 ⚠️ **TDD: Write FIRST, Ensure FAIL**

- [X] T049 [P] [US3] Integration test UpdateCharacterTests.Return_404_when_updating_deleted_character using Testcontainers in src/Tests.Integration/Acceptance/UpdateCharacterTests.cs
- [X] T050 [P] [US3] Integration test UpdateCharacterTests.Return_400_when_empty_name_provided using Testcontainers in src/Tests.Integration/Acceptance/UpdateCharacterTests.cs
- [X] T051 [P] [US3] Integration test UpdateCharacterTests.Return_400_when_invalid_guid_format using Testcontainers in src/Tests.Integration/Acceptance/UpdateCharacterTests.cs
- [X] T052 [P] [US3] Unit test CharactersShould.Return_400_when_updating_with_empty_name in src/Tests.Unit/API/CharactersShould.cs

### Implementation for User Story 3

- [X] T053 [US3] Add validation in PUT endpoint for empty/whitespace names in src/API/Characters.cs
- [X] T054 [US3] Add try-catch for ArgumentException in PUT endpoint to return 400 in src/API/Characters.cs
- [X] T055 [US3] Enhance GUID validation error messages in PUT endpoint in src/API/Characters.cs
- [X] T056 [US3] Verify all US3 tests pass (green phase of TDD)
- [X] T057 [US3] Manual test: Update non-existent character, verify clear error. Update with invalid GUID, verify clear error. Update with empty name, verify clear error.

**Checkpoint**: All user stories should now be independently functional with robust error handling

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final improvements affecting multiple user stories

- [X] T058 [P] Verify all edge cases from spec.md are covered by tests (concurrent updates, network interruption handling)
- [X] T059 [P] Code review and cleanup - ensure consistent error response format across all endpoints
- [X] T060 [P] Performance validation - verify update/delete operations complete within 2 seconds
- [X] T061 [P] Verify idempotent delete behavior is clearly documented in API responses
- [X] T062 Update API documentation with PUT and DELETE endpoints in src/API/API.http
- [X] T063 Run full test suite (unit + integration) and verify all tests pass
- [X] T064 Validate against quickstart.md scenarios to ensure implementation matches guide
- [X] T065 Update OpenAPI schema version in contracts/character-api.openapi.yaml if needed

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately (verification only)
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-5)**: All depend on Foundational phase completion
  - User Story 1 (P1) can start after Foundational
  - User Story 2 (P2) can start after Foundational (independent of US1)
  - User Story 3 (P3) can start after Foundational (independent of US1/US2)
- **Polish (Phase 6)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - Independent of US1 (different endpoints/methods)
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - Enhances US1 error handling but can be tested independently

### Within Each User Story

1. Tests MUST be written and FAIL before implementation (TDD)
2. Domain layer implementation (CharacterService, Repository)
3. Application layer implementation (Use cases)
4. API layer implementation (Endpoints)
5. DI registration
6. Verify all tests pass (green phase)
7. Manual validation

### Parallel Opportunities

- **Setup (Phase 1)**: All T001-T004 can run in parallel (verification tasks)
- **Foundational (Phase 2)**: T005 can run parallel with T006-T007
- **User Story 1 Tests**: All T008-T021 can be written in parallel (different test files)
- **User Story 2 Tests**: All T029-T041 can be written in parallel (different test files)
- **User Story 3 Tests**: All T049-T052 can be written in parallel (different test files)
- **Polish Tasks**: T058-T061 can run in parallel (different concerns)
- **Between User Stories**: Once Foundational is complete, US1, US2, and US3 can be developed in parallel by different team members

---

## Parallel Example: User Story 1 Test Phase

```bash
# Launch all tests for User Story 1 together (they will fail initially - TDD):
Task: "Unit test CharacterServiceShould.Update_character_name_successfully"
Task: "Unit test CharacterServiceShould.Return_null_when_updating_nonexistent_character"
Task: "Unit test CharacterServiceShould.Throw_exception_when_updating_with_empty_name"
Task: "Unit test MongoDbCharacterRepository.Update_returns_true_when_character_found"
Task: "Unit test MongoDbCharacterRepository.Update_returns_false_when_character_not_found"
Task: "Unit test UpdateCharacterShould.Delegate_to_character_service"
Task: "Unit test UpdateCharacterShould.Return_null_when_service_returns_null"
Task: "Unit test CharactersShould.Return_200_with_updated_character_on_successful_update"
Task: "Unit test CharactersShould.Return_404_when_updating_nonexistent_character"
Task: "Unit test CharactersShould.Return_400_when_updating_with_invalid_guid"
Task: "Integration test UpdateCharacterTests.Update_character_name_successfully"
Task: "Integration test UpdateCharacterTests.Return_404_when_character_not_found"
Task: "Integration test UpdateCharacterTests.Persist_update_across_get_requests"
```

---

## Parallel Example: Multiple User Stories

```bash
# Once Foundational phase completes, launch all three user stories in parallel:
Team Member A: Work on User Story 1 (T008-T028)
Team Member B: Work on User Story 2 (T029-T048)
Team Member C: Work on User Story 3 (T049-T057)

# Each story is independently testable and deliverable
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T004)
2. Complete Phase 2: Foundational (T005-T007) - CRITICAL checkpoint
3. Complete Phase 3: User Story 1 (T008-T028)
4. **STOP and VALIDATE**: Test User Story 1 independently
   - Create character → Update name → Verify via GET
   - Try updating non-existent character → Verify 404
   - Try updating with empty name → Verify 400
5. Deploy/demo if ready - MVP delivers update functionality

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready (T001-T007)
2. Add User Story 1 → Test independently → Deploy/Demo (MVP - Update functionality!)
3. Add User Story 2 → Test independently → Deploy/Demo (Delete functionality added!)
4. Add User Story 3 → Test independently → Deploy/Demo (Enhanced error handling!)
5. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together (T001-T007)
2. Once Foundational is done:
   - Developer A: User Story 1 (T008-T028) - Update functionality
   - Developer B: User Story 2 (T029-T048) - Delete functionality
   - Developer C: User Story 3 (T049-T057) - Error handling enhancements
3. Stories complete and integrate independently
4. Team reconvenes for Polish phase (T058-T065)

---

## Task Summary

**Total Tasks**: 65 tasks
- **Setup**: 4 tasks (verification)
- **Foundational**: 3 tasks (BLOCKING - interfaces and commands)
- **User Story 1**: 21 tasks (13 tests + 8 implementation)
- **User Story 2**: 20 tasks (13 tests + 7 implementation)
- **User Story 3**: 9 tasks (4 tests + 5 implementation)
- **Polish**: 8 tasks (validation and documentation)

**Parallel Opportunities**: 52 tasks marked [P] can run in parallel
**Test Tasks**: 30 test tasks (TDD approach)
**Implementation Tasks**: 23 implementation tasks
**Infrastructure Tasks**: 12 tasks (setup, foundational, polish)

**Estimated Effort**:
- MVP (US1 only): 6-8 hours following TDD
- Full feature (US1+US2+US3): 12-16 hours following TDD
- With parallel team (3 developers): 6-8 hours for full feature

---

## Notes

- [P] tasks = different files, no dependencies, can run in parallel
- [Story] label maps task to specific user story for traceability and independent delivery
- Each user story should be independently completable, testable, and deployable
- **TDD MANDATORY**: Verify tests fail (red) before implementing (write test → fail → implement → pass → refactor)
- Test naming follows constitution: `<ClassUnderTest>Should` with `Capital_snake_case` methods
- Commit after each logical group of tasks or at checkpoints
- Stop at any checkpoint to validate story independently
- All tests use NUnit (framework), FakeItEasy (mocking), Testcontainers (integration)
- Follow IDD architecture: API → Application → Domain → Infrastructure
- No cross-story dependencies - each story can be delivered as MVP increment
