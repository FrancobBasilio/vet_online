using Microsoft.AspNetCore.Hosting;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Servicios de sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpClient CON URL DINÁMICA (local o Azure)
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7054/api/";
builder.Services.AddHttpClient("ClinicaAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Middleware de sesión
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

//   ROTATIVA MEJORADA
try
{
    var rotativaPath = app.Environment.IsDevelopment()
        ? Path.Combine(app.Environment.ContentRootPath, "Rotativa")  // Local
        : Path.Combine(app.Environment.ContentRootPath, "Rotativa"); // Azure

    // Verificar que la ruta existe
    if (Directory.Exists(rotativaPath))
    {
        RotativaConfiguration.Setup(app.Environment.WebRootPath, rotativaPath);
        app.Logger.LogInformation($"Rotativa configurado en: {rotativaPath}");
    }
    else
    {
        app.Logger.LogWarning($"Carpeta Rotativa no encontrada en: {rotativaPath}");
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Error al configurar Rotativa");
}

app.Run();