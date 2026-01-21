using comaiz.data;
using Microsoft.EntityFrameworkCore;
using comaiz.Extensions;
using comaiz.data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using comaiz.api.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Works with appsettings.json or appsettings.[Environment].json or
// with dotnet user-secrets set "ConnectionStrings:PostgresSQL" or with environment variable
var connectionString = builder.Configuration.GetConnectionString("PostgresSQL");
Console.WriteLine($"Connection string: {connectionString}"); ;

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddDbContext<ComaizContext>(options =>
{
    options.UseNpgsql(connectionString.GetNpgsqlConnectionString());
});

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ComaizContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT SecretKey must be configured and at least 32 characters long");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// Configure CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register Token Service
builder.Services.AddScoped<ITokenService, TokenService>();

// Configure forwarded headers for reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

//add swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "comaiz.api", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                }
            },
            Array.Empty<string>()
        }
    });
});



var app = builder.Build();

// Check for command-line user creation arguments
if (args.Length >= 4 && args[0] == "--create-user")
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var username = args[1];
            var email = args[2];
            var password = args[3];
            var role = args.Length >= 5 ? args[4] : "User";
            
            Console.WriteLine($"Creating user: {username} with role: {role}");
            var success = await DatabaseSeeder.CreateUser(services, username, email, password, role);
            Environment.Exit(success ? 0 : 1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}

// Apply database migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Apply pending migrations
        var context = services.GetRequiredService<ComaizContext>();
        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");
        
        // Seed database with roles and default users (only in Development)
        var environment = services.GetRequiredService<IWebHostEnvironment>();
        var isDevelopment = environment.IsDevelopment();
        
        await DatabaseSeeder.SeedDefaultRolesAndUser(services, isDevelopment);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline.

// Use forwarded headers (must be early in pipeline)
app.UseForwardedHeaders();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "comaiz.api v1");
    options.EnableTryItOutByDefault();
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable CORS in development
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");
}

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint for CI/CD
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapControllers();

// Serve React app
app.MapFallbackToFile("index.html");

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }
