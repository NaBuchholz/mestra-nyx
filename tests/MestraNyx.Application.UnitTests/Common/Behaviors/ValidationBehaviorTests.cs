using FluentValidation;
using MestraNyx.Application.Common.Behaviors;

namespace MestraNyx.Application.UnitTests.Common.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WhenNoValidators_ShouldCallNextAndReturnResponse()
    {
        // Arrange
        var validators = Array.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(
            validators);
        var request = new TestRequest();
        var expectedResponse = new TestResponse("expected");
        var nextWasCalled = false;
        Task<TestResponse> Next(CancellationToken _)
        {
            nextWasCalled = true;
            return Task.FromResult(expectedResponse);
        }

        // Act
        var actualResponse = await behavior.Handle(
            request,
            Next,
            CancellationToken.None);

        // Assert
        Assert.True(nextWasCalled);
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public async Task Handle_WhenValidatorApprovesRequest_ShouldCallNextAndReturnResponse()
    {
        // Arrange
        var validatorWasCalled = false;
        var validator = new InlineValidator<TestRequest>();

        validator
            .RuleFor(request => request)
            .Custom((_, _) =>
            {
                validatorWasCalled = true;
            });
        var validators = new List<IValidator<TestRequest>>
        {
            validator
        };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(
            validators);
        var request = new TestRequest();
        var expectedResponse = new TestResponse("expected");
        var nextWasCalled = false;
        Task<TestResponse> Next(CancellationToken _)
        {
            nextWasCalled = true;
            return Task.FromResult(expectedResponse);
        }

        // Act
        var actualResponse = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.True(validatorWasCalled);
        Assert.True(nextWasCalled);
        Assert.Equal(expectedResponse, actualResponse);
    }

    [Fact]
    public async Task Handle_WhenValidatorRejectsRequest_ShouldThrowValidationExceptionAndNotCallNext()
    {
        // Arrange
        var validator = new InlineValidator<TestRequest>();
        validator
            .RuleFor(request => request)
            .Custom((_, context) =>
            {
                context.AddFailure("Test", "Invalid");
            });
        var validators = new List<IValidator<TestRequest>>
        {
            validator
        };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(
            validators);
        var request = new TestRequest();
        var nextWasCalled = false;
        Task<TestResponse> Next(CancellationToken _)
        {
            nextWasCalled = true;
            return Task.FromResult(new TestResponse("invalid"));
        }

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, Next, CancellationToken.None));

        // Assert
        Assert.False(nextWasCalled);
        var failure = Assert.Single(exception.Errors);
        Assert.Equal("Invalid", failure.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenMultipleValidatorsRejectRequest_ShouldAggregateFailuresAndNotCallNext()
    {
        // Arrange
        var nextWasCalled = false;
        var validator1 = new InlineValidator<TestRequest>();
        var request = new TestRequest();

        validator1
            .RuleFor(request => request)
            .Custom((_, context) =>
            {
                context.AddFailure("Name", "Name is invalid");
            });

        var validator2 = new InlineValidator<TestRequest>();

        validator2
            .RuleFor(request => request)
            .Custom((_, context) =>
            {
                context.AddFailure("OwnerId", "Owner ID is invalid");
            });

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(
            new List<IValidator<TestRequest>>
            {
                validator1,
                validator2
            });

        Task<TestResponse> Next(CancellationToken _)
        {
            nextWasCalled = true;
            return Task.FromResult(new TestResponse("invalid"));
        }

        // Act
        var validationException = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, Next, CancellationToken.None));

        // Assert
        Assert.False(nextWasCalled);
        Assert.Equal(2, validationException.Errors.Count());
        Assert.Contains("Name is invalid", validationException
            .Errors.Select(error => error.ErrorMessage));
        Assert.Contains("Name", validationException
            .Errors.Select(error => error.PropertyName));
        Assert.Contains("Owner ID is invalid", validationException
            .Errors.Select(error => error.ErrorMessage));
        Assert.Contains("OwnerId", validationException
            .Errors.Select(error => error.PropertyName));
    }

    private sealed record TestRequest;
    private sealed record TestResponse(string Value);
}
