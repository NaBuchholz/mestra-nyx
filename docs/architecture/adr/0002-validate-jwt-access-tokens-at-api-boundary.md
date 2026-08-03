# ADR 0002 — Validate JWT Access Tokens at the API Boundary

- Status: accepted
- Date: 2026-08-02

## Context

`CreateCampaignCommand` requires an `OwnerId`, but an HTTP client is not a
trusted source of campaign ownership. Accepting that identifier from the
request body would allow a caller to create a campaign on behalf of another
user.

Mestra Nyx must authenticate users without storing or validating their
passwords. The application also needs a local development mechanism before a
production identity provider is selected.

JWT and OAuth 2.0 solve different parts of this problem. JWT is a token format,
while OAuth 2.0 and OpenID Connect define standardized flows and trust
relationships for obtaining tokens and authenticating users. Generating
production access tokens inside Mestra Nyx now would create a security-sensitive
token service that the project does not need.

## Decision

The API acts only as an OAuth resource server. It does not provide password
registration, password validation, login, refresh-token, or token-issuance
endpoints.

The API authenticates bearer JWT access tokens with the official
`Microsoft.AspNetCore.Authentication.JwtBearer` middleware. Token validation
must include:

- signature;
- expected issuer;
- expected audience;
- expiration and validity period.

Inbound claim mapping is disabled so the identity contract uses the exact claim
names issued in the token. The authenticated subject comes from the `sub`
claim. During the initial implementation, `sub` must contain a non-empty
`Guid`. A missing or malformed subject is not converted into an owner identifier
and must prevent dispatch of the protected use case.

The HTTP create-campaign contract does not contain `OwnerId`. After successful
authentication, the API reads the validated subject, converts it to the
internal `Guid`, and supplies that value to `CreateCampaignCommand`. Domain and
Application remain independent of ASP.NET Core, JWT, and claims.

For local development, tokens are created with `dotnet user-jwts`. This tool is
development-only; its signing material remains in .NET User Secrets and outside
the repository. Development tokens must be short-lived and must not be
committed, copied into documentation, or written to logs.

For production, an external OpenID Connect/OAuth 2.0 provider will authenticate
users and issue delegated access tokens. The API will trust that provider by
configuration such as authority, issuer, and audience, without changing the
Domain ownership model.

If the selected provider uses a non-`Guid` subject, Mestra Nyx will introduce a
mapping from the stable external identity pair `(issuer, subject)` to an
internal user identifier. It will not hash, truncate, or otherwise coerce an
external subject into a `Guid`. This mapping stores identity references, not
passwords.

The Next.js client may obtain and send an access token, but it does not decide
whether a caller owns or may access a campaign. Authorization remains enforced
by the API.

## Dependency assessment

`Microsoft.AspNetCore.Authentication.JwtBearer` is an official ASP.NET Core
package maintained with the .NET platform. The project will use a stable version
aligned with its `net9.0` target. The package is distributed under the MIT
license.

Its maintenance cost is justified because it provides the standard ASP.NET Core
authentication handler and token-validation integration. Implementing JWT
parsing, signature validation, or authentication middleware in the project
would duplicate security-sensitive framework functionality.

## Consequences

- Mestra Nyx does not store passwords or implement credential verification.
- Clients cannot choose campaign ownership through the HTTP request body.
- Authentication details remain at the API boundary.
- The Application use case continues receiving a trusted `Guid` and remains
  independent of the token technology.
- Local development exercises the real JWT bearer middleware without requiring
  a production identity provider.
- Production requires operating or subscribing to an external identity
  provider.
- Replacing the local issuer requires configuration and security testing, even
  though the Application and Domain contracts remain unchanged.
- A provider whose subject is not a `Guid` requires an explicit external-to-
  internal identity mapping.
- Bearer tokens must be protected from disclosure because possession is
  sufficient to call the API until the token expires.

## Alternatives considered

- ASP.NET Core Identity with locally managed passwords: rejected because Mestra
  Nyx must not store or validate user passwords.
- A custom production JWT issuer inside the API: rejected because issuing and
  refreshing access tokens securely is a separate identity-provider
  responsibility and should use OAuth 2.0/OpenID Connect standards.
- Accept `OwnerId` from the request body: rejected because it enables ownership
  spoofing and mass assignment.
- Authenticate only in Next.js: rejected because callers can bypass the
  frontend and call the API directly.
- Use the external `sub` string directly as the Domain owner identifier:
  rejected for now because it couples the Domain model to one provider and does
  not safely distinguish equal subjects issued by different authorities.
- Use a custom fake authentication handler during local development: rejected
  as the default development approach because `dotnet user-jwts` can exercise
  the real bearer-token validation pipeline. Test-specific handlers may still
  be used in isolated integration-test environments when their scope is
  explicit.

## Verification

The implementation must provide automated evidence for these scenarios:

- no bearer token returns `401 Unauthorized` for campaign creation;
- a malformed, expired, incorrectly signed, wrong-issuer, or wrong-audience
  token returns `401 Unauthorized`;
- a validated token without a usable `sub` cannot dispatch campaign creation;
- a validated token whose `sub` is not a `Guid` cannot dispatch campaign
  creation;
- a valid authenticated subject becomes the command `OwnerId`;
- an `OwnerId` sent by a caller cannot override the authenticated subject;
- authentication failures do not expose token contents, signing material,
  stack traces, or private claims in responses or logs.

The production-provider migration must repeat the negative-path tests against
that provider's issuer, audience, signing keys, token lifetime, and subject
format.

## References

- [Configure JWT bearer authentication in ASP.NET Core](https://learn.microsoft.com/aspnet/core/security/authentication/configure-jwt-bearer-authentication)
- [Manage JSON Web Tokens in development with `dotnet user-jwts`](https://learn.microsoft.com/aspnet/core/security/authentication/jwt-authn)
- [RFC 9068 — JSON Web Token Profile for OAuth 2.0 Access Tokens](https://www.rfc-editor.org/rfc/rfc9068)
- [NuGet: Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)
