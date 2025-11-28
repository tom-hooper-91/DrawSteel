using System;

namespace Application;

public readonly record struct UpdateCharacterInput(Guid RouteId, string? PayloadId, string Name);
