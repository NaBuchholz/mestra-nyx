using FluentValidation;
using MediatR;

namespace MestraNyx.Application.Common.Behaviors;

/// <summary>
/// Validates MediatR requests before forwarding them to the next pipeline component.
/// </summary>
/// <typeparam name="TRequest">The type of request being validated.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the pipeline.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">The validators to execute for each request.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Executes all configured validators and forwards the request when no validation failures occur.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">The delegate for the next component in the pipeline.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>The response produced by the next pipeline component.</returns>
    /// <exception cref="ValidationException">
    /// Thrown when one or more validators report a validation failure.
    /// </exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var failures = new List<FluentValidation.Results.ValidationFailure>();
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            failures.AddRange(result.Errors);
        }
        if (failures.Any())
            throw new ValidationException(failures);

        return await next(cancellationToken);
    }
}
