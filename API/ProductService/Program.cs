using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();

// Configuración de la base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ProductService.Services.ProductService>();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product API", Version = "v1" });
});

// Configuración CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // No usar redirección HTTPS en desarrollo con Docker
    // app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Crear base de datos automáticamente
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        
        Console.WriteLine("Base de datos ProductDB creada/verificada correctamente");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
}

app.Run();