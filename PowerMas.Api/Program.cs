using Microsoft.AspNetCore.HttpOverrides;
using PowerMas.Api.Data;
using PowerMas.Api.Middlewares;
using PowerMas.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ========== Servicios ==========

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PowerMas API", Version = "v1" });
});

// CORS - Configurable por variable de entorno
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // Lee CORS_ALLOWED_ORIGINS desde ENV (formato: "https://app.netlify.app,http://localhost:5173")
        var allowedOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
        
        string[] origins;
        if (!string.IsNullOrWhiteSpace(allowedOrigins))
        {
            origins = allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
        else
        {
            // Default para desarrollo local
            origins = ["http://localhost:5173", "http://localhost:3000", "http://localhost:4200"];
        }

        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
    });
});

// Forwarded Headers (para reverse proxy como Render)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Data Access - Connection Factory
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

// Repositorios
builder.Services.AddScoped<IDocumentoIdentidadRepository, DocumentoIdentidadRepository>();
builder.Services.AddScoped<IBeneficiarioRepository, BeneficiarioRepository>();

// Servicios
builder.Services.AddScoped<IDocumentoIdentidadService, DocumentoIdentidadService>();
builder.Services.AddScoped<IBeneficiarioService, BeneficiarioService>();

var app = builder.Build();

// ========== Middleware Pipeline ==========

// Forwarded Headers (debe ir muy temprano)
app.UseForwardedHeaders();

// Manejo global de excepciones
app.UseExceptionHandling();

// Swagger: habilitado en Development, o en Production si ENABLE_SWAGGER=true
var enableSwagger = app.Environment.IsDevelopment() 
    || string.Equals(Environment.GetEnvironmentVariable("ENABLE_SWAGGER"), "true", StringComparison.OrdinalIgnoreCase);

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PowerMas API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Health check endpoint
app.MapGet("/health", () => Results.Ok("ok"))
   .WithName("HealthCheck")
   .WithTags("Health");

app.MapControllers();

app.Run();
