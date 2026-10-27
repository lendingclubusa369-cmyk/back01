using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using lender_club.Data;
using lender_club.Middleware;
using lender_club.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers and configure JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// 2. Configure PostgreSQL Database
var postgresConnection =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(postgresConnection))
{
    throw new InvalidOperationException(
        "PostgreSQL connection string 'DefaultConnection' is not configured.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(postgresConnection, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
    });
});

// Also register as AppDbContext for backward compatibility
builder.Services.AddScoped<AppDbContext>(sp =>
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(postgresConnection)
        .Options;

    return new AppDbContext(options);
});

// 3. Register Security & Business Services
builder.Services.AddSingleton<IEncryptionService, AesEncryptionService>();
builder.Services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();

// 4. Configure CORS for frontend clients
var allowedOrigins =
    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[]
    {
        "http://localhost:4200",
        "https://localhost:4200"
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 5. Configure Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lender Club - Backend API",
        Version = "v1",
        Description =
            "Dedicated Backend REST API for Identity & Loan Verification " +
            "with PostgreSQL, AES-256 PII Encryption, and PBKDF2 Password Hashing.",
        Contact = new OpenApiContact
        {
            Name = "Lender Club Backend Engineering"
        }
    });

    options.ResolveConflictingActions(
        apiDescriptions => apiDescriptions.First());

    options.CustomSchemaIds(type => type.Name);
});

var app = builder.Build();

// 6. Global Exception Handling Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// 7. Database Initialization on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        if (db.Database.IsNpgsql())
        {
            db.Database.EnsureCreated();

            logger.LogInformation(
                "PostgreSQL database connection verified and schema ensured.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "An error occurred during PostgreSQL database initialization.");
    }
}

// 8. Swagger
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Lender Club API v1");

    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Lender Club Backend API - Swagger UI";
});

// 9. Root redirect directly to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger"));





app.UseCors("FrontendCorsPolicy");

app.MapControllers();

app.Run();