# Campaigns API

The campaigns API currently exposes campaign creation at
`POST /api/campaigns`.

## Authentication and ownership

Campaign creation requires a bearer access token. After validating the token,
the API reads its `sub` claim and requires that value to be a valid GUID. That
GUID becomes the campaign owner.

`ownerId` is not part of the request contract. The API never uses a client-
supplied owner identifier to assign ownership.

## Create a campaign

```http
POST /api/campaigns
Authorization: Bearer <access-token>
Content-Type: application/json
```

### Request body

| Field | Type | Required | Rules |
| --- | --- | --- | --- |
| `name` | string | yes | Must not be empty; maximum 200 characters. |
| `system` | string or null | no | Defaults to `Call of Cthulhu 7e` when null, empty, or whitespace. |
| `description` | string or null | no | Maximum 2,000 characters. |
| `timePeriod` | string or null | no | No additional validation currently applies. |

String values are trimmed by the domain entity when the campaign is created.

```json
{
  "name": "Curse of Strahd",
  "system": "D&D 5e",
  "description": "A gothic horror campaign",
  "timePeriod": "735 BC"
}
```

### Success response

A valid request returns `201 Created` and the created campaign.

```http
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
```

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Curse of Strahd",
  "description": "A gothic horror campaign",
  "system": "D&D 5e",
  "timePeriod": "735 BC",
  "coverImage": null,
  "isPublic": false,
  "isActive": true,
  "ownerId": "11111111-1111-1111-1111-111111111111",
  "createdAt": "2026-08-04T12:00:00Z",
  "updatedAt": "2026-08-04T12:00:00Z"
}
```

The endpoint does not currently include a `Location` header because no
`GET /api/campaigns/{id}` endpoint exists. Once that read endpoint is available,
campaign creation will return its resolvable URI instead of advertising a route
that clients cannot follow.

### Validation error

An authenticated request that violates the input rules returns `400 Bad
Request` with `application/problem+json`. Error keys use the same camelCase
names as the JSON request contract.

```http
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json
```

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "instance": "/api/campaigns",
  "errors": {
    "name": [
      "'Name' must not be empty."
    ]
  }
}
```

The response may contain diagnostic extensions such as a trace identifier. API
clients should use the structured `errors` entries instead of parsing the human-
readable `title` or individual message text.

### Authentication error

The endpoint returns `401 Unauthorized` without a response body when:

- the bearer token is missing;
- the token is malformed, expired, has an invalid issuer or audience, or has an
  invalid signature;
- the validated token has no `sub` claim;
- the validated `sub` claim is not a GUID.

## Manual development requests

The executable development scenarios are available in
[`MestraNyx.API.http`](../../src/MestraNyx.API/MestraNyx.API.http). They cover
successful creation, authentication failures, ownership protection, and input
validation.

Authenticated successful requests in that file persist campaigns to the local
development database. Authentication and validation failures do not persist a
campaign.

## Related decisions

- [ADR 0001 — MediatR Validation Pipeline](../architecture/adr/0001-mediatr-validation-pipeline.md)
- [ADR 0002 — Validate JWT Access Tokens at the API Boundary](../architecture/adr/0002-validate-jwt-access-tokens-at-api-boundary.md)
- [ADR 0003 — Standardize API Validation Errors with Problem Details](../architecture/adr/0003-standardize-api-validation-errors-with-problem-details.md)
