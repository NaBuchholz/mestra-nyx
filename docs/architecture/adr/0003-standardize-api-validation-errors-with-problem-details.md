# ADR 0003 — Standardize API Validation Errors with Problem Details

- Status: accepted
- Date: 2026-08-04

## Context

ADR 0001 established that the Application layer validates MediatR requests and
throws `FluentValidation.ValidationException` before a handler runs. Application
must remain independent of HTTP, so it cannot decide which status code, media
type, or response schema represents that failure.

Without translation at the API boundary, a validation failure can surface as an
unhandled server error. Handling the exception separately in every endpoint
would duplicate a cross-cutting concern and could produce inconsistent error
contracts.

The API needs a machine-readable validation response that is safe to expose,
supports multiple messages per field, and can be reused by future endpoints.

## Decision

The API translates `FluentValidation.ValidationException` through a global
ASP.NET Core `IExceptionHandler`.

The composition root registers Problem Details services and the validation
exception handler. The exception handling middleware runs before authentication,
authorization, and endpoint execution so that it can observe downstream
validation failures.

For a recognized validation exception, the handler:

- returns `400 Bad Request`;
- uses the `application/problem+json` media type;
- produces ASP.NET Core `HttpValidationProblemDetails` through
  `TypedResults.ValidationProblem`;
- groups all messages by property name;
- converts property names to camelCase to match the JSON request contract;
- identifies the occurrence with the current request path in `instance`;
- returns `true` to indicate that the exception was handled.

For every other exception type, the handler returns `false`. Unexpected failures
must continue through the exception handling pipeline instead of being
misclassified as client errors.

The API does not expose stack traces, exception objects, authentication tokens,
or infrastructure details. Validator messages are part of the client-facing
contract and must not contain sensitive data.

## Consequences

- Application remains independent of ASP.NET Core and HTTP semantics.
- Endpoints remain focused on request mapping, authorization, dispatch, and
  success responses.
- API consumers receive a consistent, standard error representation.
- Multiple failures for the same field are preserved rather than overwritten.
- Field keys match the API's camelCase JSON convention.
- Adding another exception mapping requires another explicit handler or fallback
  policy; unknown exceptions are not silently converted to `400`.
- The API depends on ASP.NET Core Problem Details types at its presentation
  boundary, which is an intentional framework dependency.
- Property-name conversion assumes command properties correspond to request JSON
  fields. Endpoints with custom field mappings may require an explicit mapping
  strategy.
- In .NET 9, handled exceptions still emit diagnostics from the exception
  handling middleware. Validation traffic must be monitored for noisy logs and
  metrics, and the strategy may be revisited when the runtime changes.

## Alternatives considered

- Catch `ValidationException` in each endpoint: rejected because it duplicates
  response mapping and makes consistent behavior easy to omit.
- Use endpoint filters: rejected for now because filters are route-scoped and
  would require every relevant endpoint or route group to opt in. A global
  handler matches the cross-cutting nature of Application validation.
- Define a custom error envelope: rejected because Problem Details provides a
  standard media type and a well-known schema without a proprietary wrapper.
- Return validation results from every use case instead of throwing: rejected
  because it would change the MediatR request contract established by ADR 0001
  and push cross-cutting failure handling into handlers.
- Return `422 Unprocessable Content`: rejected for current input-validation
  failures. The request violates the endpoint contract before business handling,
  and `400 Bad Request` follows the established ASP.NET Core validation
  convention. A distinct domain-error policy may adopt other status codes later.

## Evidence

- `ValidationExceptionHandler` maps validation failures to the HTTP contract.
- `Program.cs` registers Problem Details, the handler, and the exception handling
  middleware.
- `CreateCampaign_WithInvalidRequest_ReturnsValidationProblem` verifies status,
  media type, camelCase field errors, and absence of persistence.
- `MestraNyx.API.http` provides a manual invalid-campaign scenario.

## Related decisions

- [ADR 0001 — MediatR Validation Pipeline](0001-mediatr-validation-pipeline.md)
- [ADR 0002 — Validate JWT Access Tokens at the API Boundary](0002-validate-jwt-access-tokens-at-api-boundary.md)

## References

- [RFC 9457 — Problem Details for HTTP APIs](https://www.rfc-editor.org/rfc/rfc9457.html)
- [Handle errors in ASP.NET Core APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling-api?view=aspnetcore-9.0)
