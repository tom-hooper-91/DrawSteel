using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace API.Validation;

public sealed class ModelStateValidationFilter : IAsyncActionFilter, IOrderedFilter
{
    private readonly ApiBehaviorOptions _options;

    public ModelStateValidationFilter(IOptions<ApiBehaviorOptions> options)
    {
        _options = options.Value;
    }

    public int Order => -1000;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var hasCharacterValidator = context.Filters.Any(filter => filter is CharacterRequestValidatorAttribute);

        if (!hasCharacterValidator && !context.ModelState.IsValid)
        {
            context.Result = _options.InvalidModelStateResponseFactory(context);
            return;
        }

        await next();
    }
}
