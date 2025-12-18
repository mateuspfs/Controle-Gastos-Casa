using System.Collections.Concurrent;
using ControleGastosCasa.Application.Services.Interfaces;

namespace ControleGastosCasa.Application.Services.Mapping;

/// Mapper simples com registro manual de funções de conversão.
public class SimpleMapper : IMapper
{
    private readonly ConcurrentDictionary<(Type Source, Type Destination), Delegate> _maps = new();

    public void Register<TSource, TDestination>(Func<TSource, TDestination> map)
    {
        _maps[(typeof(TSource), typeof(TDestination))] = map ?? throw new ArgumentNullException(nameof(map));
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (!_maps.TryGetValue((typeof(TSource), typeof(TDestination)), out var del))
            throw new InvalidOperationException($"Nenhum mapeamento registrado para {typeof(TSource).Name} -> {typeof(TDestination).Name}");

        return ((Func<TSource, TDestination>)del)(source);
    }
}

