using ControleGastosCasa.Application;
using ControleGastosCasa.Infrastructure;
using ControleGastosCasa.Infrastructure.Persistence;
using ControleGastosCasa.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Controle de Gastos da Casa API",
        Version = "v1",
        Description = "API RESTful para gerenciamento de gastos domésticos. Permite o controle de transações financeiras, categorização de gastos e gerenciamento de pessoas, oferecendo uma visão clara e organizada das finanças domésticas.",
    });

    // Inclui comentários XML na documentação
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// Configuração de CORS para permitir requisições do frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Aplica migrações automaticamente na inicialização
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}

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

// Classe parcial para permitir que WebApplicationFactory funcione com top-level programs
namespace ControleGastosCasa.Api
{
    public partial class Program
    {
    }
}
