using MestraNyx.Application;
using MestraNyx.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MestraNyx.API.Features.Campaigns.CreateCampaign;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is missing or blank.");
}

builder.Services
    .AddApplication()
    .AddInfrastructure(connectionString);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
    });

builder.Services
    .AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapCreateCampaignEndpoint();

app.Run();

public partial class Program
{
}
