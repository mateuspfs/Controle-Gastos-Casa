using ControleGastosCasa.Application;
using ControleGastosCasa.Infrastructure;
using ControleGastosCasa.Infrastructure.Persistence;
using ControleGastosCasa.Infrastructure.Seeders;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ControleGastosCasa API",
        Version = "v1"
    });
});

// Configuração de CORS para permitir requisições do frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:5173";
        policy
            .WithOrigins(frontendUrl, "http://localhost:5173", "http://frontend:80")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Executa seeders automaticamente se as tabelas estiverem vazias
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await PessoaSeeder.SeedAsync(context);
    await CategoriaSeeder.SeedAsync(context);
    await TransacaoSeeder.SeedAsync(context);
}

app.UseSwagger();
app.UseSwaggerUI();

// Habilita CORS antes de outros middlewares
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
