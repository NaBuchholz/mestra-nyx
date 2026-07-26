using FluentValidation;
using MediatR;

namespace MestraNyx.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationTasks = _validators.Select(v => v.ValidateAsync(request, cancellationToken));

        var validationResults = await Task.WhenAll(validationTasks);

        var failures = validationResults.SelectMany(r => r.Errors).ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next(cancellationToken);
    }
}
