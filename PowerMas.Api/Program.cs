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

// CORS - Permisivo para desarrollo (ajustar en producción)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
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

// Manejo global de excepciones (debe ir primero)
app.UseExceptionHandling();

// Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PowerMas API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
