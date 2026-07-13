using System.Text;
using Andromeda.Data;
using Andromeda.Exceptions;
using Andromeda.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


#region Serilog

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

#endregion

#region OpenApi

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT"
        });

        document.Security = new List<OpenApiSecurityRequirement>
        {
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = new List<string>()
                }
        };

        return Task.CompletedTask;
    });
});

#endregion

#region Problem Details

builder.Services.AddProblemDetails();

#endregion

#region Authentication And Authorization

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var jwtConfig = builder.Configuration.GetSection("JWTConfig");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtConfig["SecretKey"]!)
        ),
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddAuthorization();

#endregion

#region EF Core

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
});

#endregion

#region ExceptionHandler

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

#endregion

#region Add FluentValidation Validators
    builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Andromeda API")
            .ShowOperationId()
            .SortTagsAlphabetically()
            .SortOperationsByMethod();
    });
}

app.UseSerilogRequestLogging();


app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();


app.UseExceptionHandler();

app.Map("/", () => "Hello from koorosh again");
app.MapEndpoints();

app.Run();