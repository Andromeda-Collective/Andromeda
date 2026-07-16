using System.Text;
using Andromeda.Data;
using Andromeda.Entities;
using Andromeda.Exceptions;
using Andromeda.Extensions;
using Andromeda.Features.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

#region Add Identity
    
    builder.Services
        .AddIdentityCore<User>(options =>
        {
            options.Password.RequiredLength = 8;

            options.Password.RequireDigit = false;

            options.Password.RequireUppercase = false;

            options.Password.RequireLowercase = false;

            options.Password.RequireNonAlphanumeric = false;

            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedAccount = false;

            options.SignIn.RequireConfirmedEmail = false;

            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddRoles<Role>()
        .AddSignInManager()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

#endregion

#region Add Services
    
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();

#endregion

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.ApplyMigrationsAsync();
    await SeedData.SeedAsync(scope.ServiceProvider);
}


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


app.MapEndpoints();

app.Run();