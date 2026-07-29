# ADR 0001 — MediatR Validation Pipeline

- Status: accepted
- Date: 2026-07-29

## Context

Registering validators with dependency injection does not make MediatR execute
them automatically. Calling each validator from within handlers would duplicate
a cross-cutting responsibility and allow new use cases to be implemented
without validation.

## Decision

The Application layer uses a `ValidationBehavior` in the MediatR pipeline.
Before the handler runs, the behavior executes every validator registered for
the request, aggregates their failures, and throws `ValidationException` when
the input is invalid.

Validators run sequentially. Parallel execution could reduce latency, but
scoped dependencies used by validators, such as a `DbContext`, are not
necessarily thread-safe. We prioritize safety and predictable behavior.

The behavior remains independent of ASP.NET Core and Infrastructure. The API
will be responsible for converting the exception into a consistent HTTP
response.

## Consequences

- Handlers remain focused on use cases.
- Validation is applied consistently to requests sent through
  MediatR.
- Failures from multiple validators are returned together.
- Slow validators increase total request time because they run sequentially.
- Direct calls to a handler do not automatically pass through the pipeline.

## Alternatives considered

- Validate inside each handler: rejected because it duplicates logic and makes
  validation easy to omit.
- Validate in the API: rejected because it couples validation to ASP.NET Core
  and does not protect other entry points.
- Run validators in parallel: rejected because of the risk of concurrent use
  of scoped dependencies that are not thread-safe.

## Evidence

`ValidationBehaviorTests` covers valid and invalid requests and failure
aggregation. `CreateCampaignValidatorTests` covers the input rules specific to
campaign creation.
