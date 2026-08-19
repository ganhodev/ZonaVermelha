using Microsoft.EntityFrameworkCore;
using ZonaVermelha.API.BackgroundServices;
using ZonaVermelha.API.Hubs;
using ZonaVermelha.API.Middlewares;
using ZonaVermelha.API.Services;
using ZonaVermelha.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHostedService<ZonaExpiracaoService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ZonaVermelhaDbContext>(options => options.UseSqlite(connectionString));

//Scoped: cria uma instância nova por requisição HTTP. É o padrão pra serviços que usam DbContext.
builder.Services.AddScoped<RelatoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ZonaService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // necessário pro SignalR
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();
app.MapHub<ZonasHub>("/hubs/zonas");

app.Run();
