using Guess.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ===== SERVICE CONFIGURATION =====
// Core infrastructure services
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddIdentityServices();

// Authentication and Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationServices();

// API features
builder.Services.AddApiVersioningServices();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCorsServices(builder.Environment);

// Application services and validation
builder.Services.AddApplicationServices();
builder.Services.AddFluentValidationServices();
builder.Services.AddBackgroundServices();

// Health checks and configuration
builder.Services.AddHealthCheckServices();
builder.Services.AddConfigurationServices(builder.Configuration);

// Controller services with JSON configuration
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

var app = builder.Build();

// ===== MIDDLEWARE PIPELINE CONFIGURATION =====
// Validate configuration at startup
app.ValidateConfiguration();

// Configure complete application pipeline
app.UseApplicationPipeline(app.Environment);

// Add health check endpoints
app.UseHealthCheckEndpoints();

// ===== STATIC FILES CONFIGURATION FOR SPA =====
// Serve static files from wwwroot (Angular build output)
app.UseDefaultFiles(); // Serves index.html as default
app.UseStaticFiles();

// SPA fallback - redirect all non-API routes to index.html for Angular routing
app.MapFallbackToFile("/index.html");

// Initialize database
await app.InitializeDatabaseAsync();

app.Run();