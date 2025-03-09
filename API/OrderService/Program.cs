using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de puerto explícito para Docker
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Asegúrate de escuchar en el puerto 80
});

// Configuración de CORS
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

// Configuración de sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<OrderServices>();
builder.Services.AddScoped<CartService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();  
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
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
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Crear base de datos automáticamente
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        context.Database.EnsureCreated();
        
        // Si necesitas aplicar migraciones pendientes:
        // context.Database.Migrate();
        
        Console.WriteLine("Base de datos OrderDB creada/verificada correctamente");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
}

app.Run();