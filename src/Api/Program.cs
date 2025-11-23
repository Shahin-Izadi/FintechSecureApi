
using Application.Common.Options;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

builder.Services.AddScoped<Application.Interfaces.IAuthService, Infrastructure.Services.AuthService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;  // ← THIS LINE IS REQUIRED ON AZURE
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FintechSecureApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,          // <-- IMPORTANT
        Scheme = "bearer",                       // <-- IMPORTANT (lowercase)
        BearerFormat = "JWT"                     // <-- IMPORTANT
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "bearer",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();
// ADD THIS BLOCK – shows exactly why JWT fails . due to some fucking mistake I made I had to add this to see why the 
// was I getting 401 in authentication . 
//app.Use(async (context, next) =>
//{
//    Console.WriteLine("======== JWT DEBUG START ========");

//    // 1. Log Authorization header
//    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
//        Console.WriteLine($"Authorization Header: {authHeader}");
//    else
//        Console.WriteLine("Authorization Header: NONE");

//    // 2. Log raw token if present
//    var headerValue = authHeader.ToString();
//    if (headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
//    {
//        var rawToken = headerValue.Substring("Bearer ".Length).Trim();
//        Console.WriteLine($"Raw JWT Token: {rawToken}");
//    }
//    else
//    {
//        Console.WriteLine("No Bearer token in Authorization header");
//    }

//    // 3. Try manual authenticate and log every detail
//    var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

//    if (!authResult.Succeeded)
//    {
//        Console.WriteLine("JWT AUTH FAILED");

//        if (authResult.Failure != null)
//        {
//            Console.WriteLine($"- Failure Message: {authResult.Failure.Message}");
//            Console.WriteLine($"- Exception Type: {authResult.Failure.GetType().FullName}");
//            Console.WriteLine($"- Inner Exception: {authResult.Failure.InnerException?.Message}");
//            Console.WriteLine($"- Stack Trace:\n{authResult.Failure.StackTrace}");
//        }
//        else
//        {
//            Console.WriteLine("- Failure: No exception (auth failed silently)");
//        }
//    }
//    else
//    {
//        Console.WriteLine("JWT AUTH SUCCEEDED");

//        if (context.User?.Identity != null)
//            Console.WriteLine($"- Identity: {context.User.Identity.Name}");
//        else
//            Console.WriteLine("- Identity: null");

//        Console.WriteLine("- Claims:");
//        foreach (var claim in context.User.Claims)
//            Console.WriteLine($"    {claim.Type} = {claim.Value}");
//    }

//    // 4. Log authentication scheme used
//    Console.WriteLine($"Authentication Scheme: {JwtBearerDefaults.AuthenticationScheme}");

//    // 5. Log endpoint about to be hit
//    var endpoint = context.GetEndpoint();
//    Console.WriteLine($"Endpoint: {endpoint?.DisplayName ?? "None"}");

//    Console.WriteLine("========= JWT DEBUG END =========\n");

//    await next();
//});
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FintechSecureApi v1");
    c.RoutePrefix = string.Empty; // <-- Swagger at root URL: https://yourapp.azurewebsites.net/
});
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();