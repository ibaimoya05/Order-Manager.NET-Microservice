using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ProductCrudBlazor.Services;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-ES");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
});

// Registrar HttpClient para los servicios
builder.Services.AddHttpClient<ProductService>(client =>
{
    var apiUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") 
        ?? throw new InvalidOperationException("API Base URL not configured");
    client.BaseAddress = new Uri(apiUrl);
});

// Asegúrate de que existan estos registros:

builder.Services.AddHttpClient<ProductCrudBlazor.Services.ProductService>();
builder.Services.AddHttpClient<ProductCrudBlazor.Services.CartService>();
builder.Services.AddHttpClient<ProductCrudBlazor.Services.OrderService>();


builder.Services.AddScoped<ProductCrudBlazor.Services.ProductService>();
builder.Services.AddScoped<ProductCrudBlazor.Services.CartService>();
builder.Services.AddScoped<ProductCrudBlazor.Services.OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();