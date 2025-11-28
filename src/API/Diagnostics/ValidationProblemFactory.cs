using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Diagnostics;

public static class ValidationProblemFactory
{
    private const string DefaultTitle = "Request validation failed";

    public static ValidationProblemDetails InvalidIdentifier(string detail)
    {
        var errors = new Dictionary<string, string[]>
        {
            ["id"] = new[] { detail }
        };

        return Create(details: detail, errors: errors);
    }

    public static ValidationProblemDetails InvalidPayload(string detail, Dictionary<string, string[]>? errors = null)
    {
        return Create(detail, errors ?? new Dictionary<string, string[]>());
    }

    private static ValidationProblemDetails Create(string details, Dictionary<string, string[]> errors)
    {
        return new ValidationProblemDetails(errors)
        {
            Title = DefaultTitle,
            Detail = details,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
    }

    public static IActionResult ToResult(this ProblemDetails details)
    {
        var statusCode = details.Status ?? StatusCodes.Status400BadRequest;
        if (statusCode == StatusCodes.Status400BadRequest)
        {
            return new BadRequestObjectResult(details);
        }

        return new ObjectResult(details) { StatusCode = statusCode };
    }
}
