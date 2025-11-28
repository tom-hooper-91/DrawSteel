using System.Linq;
using API.Requests;
using API.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Tests.API;

[TestFixture]
public class CharacterRequestValidatorShould
{
    [Test]
    public void Reject_legacy_identifier_payloads()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("request.id", "The JSON value could not be converted to System.String.");
        var request = new CharacterRequest { Name = "Frodo" };

        var result = CharacterRequestValidator.Validate(
            request,
            CharacterRequestValidationContext.ForCreate(modelState));

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Errors.ContainsKey("id"), Is.True);
        Assert.That(result.Errors["id"].Single(), Does.Contain("Legacy identifier payloads"));
    }

    [Test]
    public void Return_problem_details_for_name_validation_errors()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("request.name", "The 'name' field is required.");
        var request = new CharacterRequest { Name = string.Empty };

        var result = CharacterRequestValidator.Validate(
            request,
            CharacterRequestValidationContext.ForCreate(modelState));

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Errors.ContainsKey("name"), Is.True);
        Assert.That(result.Errors["name"].Single(), Is.EqualTo("The 'name' field is required."));
    }

    [Test]
    public void Require_identifier_when_validator_requests_it()
    {
        var modelState = new ModelStateDictionary();
        var routeId = Guid.NewGuid();
        var request = new CharacterRequest { Name = "Sam" };

        var result = CharacterRequestValidator.Validate(
            request,
            CharacterRequestValidationContext.ForUpdate(modelState, routeId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Errors.ContainsKey("id"), Is.True);
        Assert.That(result.Errors["id"].Single(), Does.Contain("required"));
    }

    [Test]
    public void Reject_invalid_identifier_formats()
    {
        var modelState = new ModelStateDictionary();
        var routeId = Guid.NewGuid();
        var request = new CharacterRequest { Id = "not-a-guid", Name = "Sam" };

        var result = CharacterRequestValidator.Validate(
            request,
            CharacterRequestValidationContext.ForUpdate(modelState, routeId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Errors.ContainsKey("id"), Is.True);
        Assert.That(result.Errors["id"].Single(), Does.Contain("valid GUID"));
    }

    [Test]
    public void Reject_mismatched_route_identifier()
    {
        var modelState = new ModelStateDictionary();
        var routeId = Guid.NewGuid();
        var request = new CharacterRequest { Id = Guid.NewGuid().ToString(), Name = "Pippin" };

        var result = CharacterRequestValidator.Validate(
            request,
            CharacterRequestValidationContext.ForUpdate(modelState, routeId));

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Errors.ContainsKey("id"), Is.True);
        Assert.That(result.Errors["id"].Single(), Does.Contain("match the route"));
    }

    [Test]
    public void Allow_scalar_identifier_for_optional_flows()
    {
        var modelState = new ModelStateDictionary();
        var request = new CharacterRequest { Id = Guid.NewGuid().ToString(), Name = "Merry" };

        var result = CharacterRequestValidator.Validate(
            request,
            CharacterRequestValidationContext.ForCreate(modelState));

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Allow_matching_route_identifier()
    {
        var modelState = new ModelStateDictionary();
        var routeId = Guid.NewGuid();
        var request = new CharacterRequest { Id = routeId.ToString(), Name = "Aragorn" };

        var result = CharacterRequestValidator.Validate(
            request,
            CharacterRequestValidationContext.ForUpdate(modelState, routeId));

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Normalize_pointer_style_model_state_keys()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("$.id", "Legacy error");
        var request = new CharacterRequest { Name = "Bilbo" };

        var result = CharacterRequestValidator.Validate(
            request,
            CharacterRequestValidationContext.ForCreate(modelState));

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Errors.ContainsKey("id"), Is.True);
    }
}
