using System.Text;
using gateway_api;
using gateway_api.Clients;
using gateway_api.Models.Auth;
using gateway_api.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(
        o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    )
    .AddJwtBearer(
        o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                ValidAudience = builder.Configuration["JwtConfig:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
        }
    );

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(
    options =>
    {
        options.EnableAnnotations();
        options.CustomSchemaIds(SwaggerTypeNamesProvider.GetSwaggerDisplayedName);
        options.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Version = "v1",
                Title = "gateway-api",
                Description = "gateway for weather"
            }
        );
        options.MapType<TimeSpan>(
            () => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("00:00")
            }
        );
        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme.<br/><br/>
                          Enter 'Bearer' [space] and then your token in the text input below.<br/><br/>
                          Example: 'Bearer 12345abcdef'",
                Name = HeaderNames.Authorization,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            }
        );

        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            }
        );
    }
);

var externalServicesSection = builder.Configuration.GetSection(nameof(ExternalServicesOptions));
builder.Services.Configure<ExternalServicesOptions>(externalServicesSection);

builder.Services.AddHttpClient<WeatherHttpClient>(
    (provider, client) =>
    {
        var options = provider.GetRequiredService<IOptions<ExternalServicesOptions>>();
        var baseUrl = options.Value.WeatherServiceUrl ?? throw new InvalidOperationException("WeatherService URL was not configured");
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(5);
    }
);
builder.Services.AddHttpClient<SalaryHttpClient>(
    (provider, client) =>
    {
        var options = provider.GetRequiredService<IOptions<ExternalServicesOptions>>();
        var baseUrl = options.Value.SalaryServiceUrl ?? throw new InvalidOperationException("SalaryService URL was not configured");
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(5);
    }
);
builder.Services.AddHttpClient<AuthHttpClient>(
    (provider, client) =>
    {
        var options = provider.GetRequiredService<IOptions<ExternalServicesOptions>>();
        var baseUrl = options.Value.AuthServiceUrl ?? throw new InvalidOperationException("AuthService URL was not configured");
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(5);
    }
);

var app = builder.Build();

app.UseHealthChecks("/health-check");

app.UseSwagger(
    options => { options.RouteTemplate = "swagger/{documentName}/swagger.json"; }
);
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/weather/get", (WeatherHttpClient weatherClient, CancellationToken cancellationToken) => weatherClient.GetWeather(cancellationToken))
    .RequireAuthorization();
app.MapGet("/salary/get", (SalaryHttpClient salaryClient, CancellationToken cancellationToken) => salaryClient.GetSalary(cancellationToken))
    .RequireAuthorization();
app.MapPost("/auth/login",
    async ([FromBody] Auth.Request request, AuthHttpClient authClient, CancellationToken cancellationToken) =>
    {
        var result = await authClient.Auth(request, cancellationToken);
        return result is null
            ? Results.Unauthorized()
            : Results.Ok(result);
    }
);

app.Run();