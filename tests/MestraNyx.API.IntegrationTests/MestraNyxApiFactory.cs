using System.Text;
using MestraNyx.API.IntegrationTests.Features.Campaigns.CreateCampaign;
using MestraNyx.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace MestraNyx.API.IntegrationTests;

public sealed class MestraNyxApiFactory : WebApplicationFactory<Program>
{
    internal const string TestIssuer = "mestra-nyx-integration-tests";

    internal const string TestAudience = "mestra-nyx-api-integration-tests";

    internal static SymmetricSecurityKey TestSigningKey { get; } = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes("mestra-nyx-integration-tests-signing-key"));

    internal FakeCampaignRepository CampaignRepository { get; } = new FakeCampaignRepository();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            "Host=localhost;Database=mestra_nyx_tests;Username=test;Password=test");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ICampaignRepository>();
            services.AddSingleton<ICampaignRepository>(CampaignRepository);
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = TestSigningKey,
                        ValidateIssuer = true,
                        ValidIssuer = TestIssuer,
                        ValidateAudience = true,
                        ValidAudience = TestAudience,
                        ValidateLifetime = true,
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        });
    }
}
