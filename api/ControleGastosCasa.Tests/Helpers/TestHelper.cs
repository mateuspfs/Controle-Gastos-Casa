using AutoMapper;
using ControleGastosCasa.Application.Mapping;

namespace ControleGastosCasa.Tests.Helpers;

// Classe auxiliar para configuração de testes
public static class TestHelper
{
    // Cria uma instância do AutoMapper configurada para os testes
    public static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DomainToDtoMapping>();
            cfg.AddProfile<DtoToDomainMapping>();
        });
        
        configuration.AssertConfigurationIsValid();
        return configuration.CreateMapper();
    }
}