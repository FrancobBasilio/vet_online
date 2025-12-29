var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  para que el frontend pueda consumir la API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7001",  // Para desarrollo local
                "https://mivet-webapp-g9hwcmdghwf0bqde.canadacentral-01.azurewebsites.net"  // Para producción en Azure
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// SWAGGER EN PRODUCCIÓN (útil para debugging en Azure)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//  CORS antes de Authorization
app.UseCors("AllowWebApp");

app.UseAuthorization();

app.MapControllers();

app.Run();