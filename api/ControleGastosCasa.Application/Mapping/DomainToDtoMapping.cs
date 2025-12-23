using AutoMapper;
using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Helpers;
using ControleGastosCasa.Domain.Entities;

namespace ControleGastosCasa.Application.Mapping
{
    public class DomainToDtoMapping : Profile
    {
        // Mapeamentos de entidades de domínio para DTOs
        public DomainToDtoMapping()
        {
            CreateMap<Pessoa, PessoaDto>();
            CreateMap<Categoria, CategoriaDto>();
            CreateMap<Transacao, TransacaoDto>();
        }
    }
}
