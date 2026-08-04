using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace MestraNyx.API.ExceptionHandlers;

internal sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        var errors = validationException.Errors
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => JsonNamingPolicy.CamelCase.ConvertName(group.Key),
                group => group.Select(
                    failure => failure.ErrorMessage).ToArray()
                );

        var validationProblemResult = TypedResults.ValidationProblem(errors: errors, instance: httpContext.Request.Path.Value);

        await validationProblemResult.ExecuteAsync(httpContext);

        return true;
    }
}