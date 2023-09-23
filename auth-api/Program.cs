using auth_api.Jwt;
using auth_api.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

var app = builder.Build();

app.MapPost(
    "/auth",
    ([FromBody] AuthInput input, [FromServices] IJwtTokenGenerator jwtGenerator) =>
    {
        if (input is {Login: "admin", Password: "admin"})
        {
            var token = jwtGenerator.GenerateToken(input.Login);
            return Results.Ok(token);
        }

        return Results.Unauthorized();
    }
);

app.Run();