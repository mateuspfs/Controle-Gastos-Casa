using AutoMapper;
using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Domain.Entities;

namespace ControleGastosCasa.Application.Mapping
{
    public class DtoToDomainMapping : Profile
    {
        public DtoToDomainMapping()
        {
            CreateMap<PessoaDto, Pessoa>();
            CreateMap<CategoriaDto, Categoria>();
            CreateMap<TransacaoDto, Transacao>();
        }
    }
}
