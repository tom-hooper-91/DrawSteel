using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using API.Diagnostics;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Validation;

public sealed record CharacterRequestValidationContext(
    ModelStateDictionary ModelState,
    bool RequireIdentifier,
    bool EnsureRouteIdentifierMatches,
    Guid? RouteIdentifier)
{
    public static CharacterRequestValidationContext ForCreate(ModelStateDictionary modelState) =>
        new(modelState, RequireIdentifier: false, EnsureRouteIdentifierMatches: false, RouteIdentifier: null);

    public static CharacterRequestValidationContext ForUpdate(ModelStateDictionary modelState, Guid routeIdentifier) =>
        new(modelState, RequireIdentifier: true, EnsureRouteIdentifierMatches: true, RouteIdentifier: routeIdentifier);
}

public static class CharacterRequestValidator
{
    private const string LegacyIdentifierMessage = "Legacy identifier payloads are not supported; send 'id' as a string value.";
    private const string InvalidPayloadDetail = "Payload validation failed";
    private const string MissingIdentifierMessage = "The 'id' field is required for this operation.";
    private const string InvalidIdentifierMessage = "The 'id' field must be a valid GUID.";
    private const string MismatchedIdentifierMessage = "The 'id' field must match the route identifier.";

    public static ValidationProblemDetails? Validate(CharacterRequest? request, CharacterRequestValidationContext context)
    {
        NormalizeLegacyIdentifierErrors(context.ModelState);

        var legacyIdentifierProblem = TryResolveLegacyIdentifierProblem(context.ModelState);
        if (legacyIdentifierProblem is not null)
        {
            return legacyIdentifierProblem;
        }

        if (!context.ModelState.IsValid)
        {
            var errors = BuildProblemErrors(context.ModelState);
            return ValidationProblemFactory.InvalidPayload(InvalidPayloadDetail, errors);
        }

        return request is null ? null : ValidateIdentifiers(request, context);
    }

    private static ValidationProblemDetails? ValidateIdentifiers(CharacterRequest request, CharacterRequestValidationContext context)
    {
        var identifier = string.IsNullOrWhiteSpace(request.Id) ? null : request.Id!.Trim();

        if (context.RequireIdentifier && string.IsNullOrWhiteSpace(identifier))
        {
            return ValidationProblemFactory.InvalidIdentifier(MissingIdentifierMessage);
        }

        if (identifier is null)
        {
            return null;
        }

        if (!Guid.TryParse(identifier, out var parsedIdentifier))
        {
            return ValidationProblemFactory.InvalidIdentifier(InvalidIdentifierMessage);
        }

        if (context.EnsureRouteIdentifierMatches && context.RouteIdentifier is Guid routeIdentifier && routeIdentifier != parsedIdentifier)
        {
            return ValidationProblemFactory.InvalidIdentifier(MismatchedIdentifierMessage);
        }

        return null;
    }

    private static void NormalizeLegacyIdentifierErrors(ModelStateDictionary modelState)
    {
        foreach (var entry in modelState)
        {
            if (!IsIdentifierKey(entry.Key) || entry.Value is null || entry.Value.Errors.Count == 0)
            {
                continue;
            }

                if (entry.Value.Errors.Any(error =>
                    error.Exception is JsonException ||
                    error.ErrorMessage.Contains("System.Text.Json", StringComparison.OrdinalIgnoreCase) ||
                    error.ErrorMessage.Contains("JSON value could not be converted", StringComparison.OrdinalIgnoreCase)))
            {
                entry.Value.Errors.Clear();
                entry.Value.Errors.Add(LegacyIdentifierMessage);
            }
        }
    }

    private static Dictionary<string, string[]> BuildProblemErrors(ModelStateDictionary modelState)
    {
        return modelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .ToDictionary(
                entry => NormalizeKey(entry.Key),
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());
    }

    private static ValidationProblemDetails? TryResolveLegacyIdentifierProblem(ModelStateDictionary modelState)
    {
        var entriesWithErrors = modelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .ToList();

        if (entriesWithErrors.Count == 0)
        {
            return null;
        }

        var allErrorsAreLegacyIdentifier = entriesWithErrors.All(entry =>
            IsIdentifierKey(entry.Key) &&
            entry.Value!.Errors.All(error =>
                string.Equals(error.ErrorMessage, LegacyIdentifierMessage, StringComparison.Ordinal)));

        return allErrorsAreLegacyIdentifier
            ? ValidationProblemFactory.InvalidIdentifier(LegacyIdentifierMessage)
            : null;
    }

    private static bool IsIdentifierKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        var normalized = NormalizeKey(key);
        return string.Equals(normalized, "id", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }

        var sanitized = key.TrimStart('$');
        var separators = new[] { '.', '[', ']' };
        var tokens = sanitized.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        var token = tokens.Length > 0 ? tokens[^1] : sanitized;

        if (string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        return token.Length == 1 ? token.ToLowerInvariant() : char.ToLowerInvariant(token[0]) + token[1..];
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class CharacterRequestValidatorAttribute : Attribute, IAsyncActionFilter, IOrderedFilter
{
    public bool RequireIdentifier { get; init; }
    public bool EnsureRouteMatches { get; init; }
    public string RouteIdentifierArgumentName { get; init; } = "characterId";
    public int Order => int.MinValue;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        TryResolveRequest(context.ActionArguments, out var request);

        var routeId = ExtractRouteIdentifier(context.ActionArguments, RouteIdentifierArgumentName);
        var validationContext = new CharacterRequestValidationContext(
            context.ModelState,
            RequireIdentifier,
            EnsureRouteMatches,
            routeId);

        var problem = CharacterRequestValidator.Validate(request, validationContext);
        if (problem is not null)
        {
            context.Result = problem.ToResult();
            return;
        }

        await next();
    }

    private static bool TryResolveRequest(IDictionary<string, object?> arguments, out CharacterRequest request)
    {
        foreach (var argument in arguments.Values)
        {
            if (argument is CharacterRequest typedRequest)
            {
                request = typedRequest;
                return true;
            }
        }

        request = null!;
        return false;
    }

    private static Guid? ExtractRouteIdentifier(IDictionary<string, object?> arguments, string routeArgumentName)
    {
        if (!arguments.TryGetValue(routeArgumentName, out var value) || value is null)
        {
            return null;
        }

        return value switch
        {
            Guid guid => guid,
            string text when Guid.TryParse(text, out var parsed) => parsed,
            _ => null
        };
    }
}
