using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MestraNyx.API.IntegrationTests;
using Microsoft.IdentityModel.Tokens;

namespace MestraNyx.API.IntegrationTests.Authentication;

internal static class TestJwtTokenFactory
{
    internal static string CreateToken(
        string? subject = null,
        string? issuer = null,
        string? audience = null,
        DateTime? notBefore = null,
        DateTime? expires = null,
        SecurityKey? signingKey = null)
    {
        var now = DateTime.UtcNow;

        var claims = new List<Claim>();

        if (subject is not null)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, subject));
        }

        var signingCredentials = new SigningCredentials(
            signingKey ?? MestraNyxApiFactory.TestSigningKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer ?? MestraNyxApiFactory.TestIssuer,
            audience: audience ?? MestraNyxApiFactory.TestAudience,
            claims: claims,
            notBefore: notBefore ?? now.AddMinutes(-1),
            expires: expires ?? now.AddMinutes(5),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
