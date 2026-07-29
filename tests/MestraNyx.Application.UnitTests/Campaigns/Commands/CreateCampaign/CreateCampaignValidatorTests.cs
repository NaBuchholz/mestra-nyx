using FluentValidation;
using MestraNyx.Application.Campaigns.Commands.CreateCampaign;

namespace MestraNyx.Application.UnitTests.Campaign.Commands.CreateCampaign;

public sealed class CreateCampaignValidatorTests
{
    [Fact]
    public void Validate_WhenCommandIsValid_ShouldHaveNoErrors()
    {
        // Arrange
        var name = "Horror no expresso do Oriente";
        var ownerId = Guid.NewGuid();
        var command = new CreateCampaignCommand(name, null, null, null, ownerId);
        var validator = new CreateCampaignValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WhenNameAndOwnerIdAreEmpty_ShouldReturnExpectedErrors()
    {
        // Arrange
        var name = string.Empty;
        var ownerId = Guid.Empty;
        var command = new CreateCampaignCommand(name, null, null, null, ownerId);
        var validator = new CreateCampaignValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateCampaignCommand.Name));
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateCampaignCommand.OwnerId));
    }

    [Fact]
    public void Validate_WhenNameExceedsMaximumLength_ShouldReturnNameError()
    {
        // Arrange
        var name = new string('a', 201);
        var command = new CreateCampaignCommand(
            name,
            null,
            null,
            null,
            Guid.NewGuid());
        var validator = new CreateCampaignValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateCampaignCommand.Name));
    }

    [Fact]
    public void Validate_WhenDescriptionExceedsMaximumLength_ShouldReturnDescriptionError()
    {
        // Arrange
        var description = new string('a', 2001);
        var command = new CreateCampaignCommand(
            "Horror no Expresso do Oriente",
            null,
            description,
            null,
            Guid.NewGuid());
        var validator = new CreateCampaignValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(CreateCampaignCommand.Description));
    }

    [Fact]
    public void Validate_WhenOptionalFieldsAreNull_ShouldHaveNoErrors()
    {
        // Arrange
        var command = new CreateCampaignCommand(
            "Horror no Expresso do Oriente",
            null,
            null,
            null,
            Guid.NewGuid());
        var validator = new CreateCampaignValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
