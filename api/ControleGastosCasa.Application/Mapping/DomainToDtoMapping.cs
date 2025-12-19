using AutoMapper;
using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Domain.Entities;

namespace ControleGastosCasa.Application.Mapping
{
    public class DomainToDtoMapping : Profile
    {
        public DomainToDtoMapping()
        {
            CreateMap<Pessoa, PessoaDto>();
            CreateMap<Categoria, CategoriaDto>();
            CreateMap<Transacao, TransacaoDto>();
        }
    }
}
