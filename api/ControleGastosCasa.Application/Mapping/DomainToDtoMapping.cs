using AutoMapper;
using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Helpers;
using ControleGastosCasa.Domain.Entities;

namespace ControleGastosCasa.Application.Mapping
{
    public class DomainToDtoMapping : Profile
    {
        public DomainToDtoMapping()
        {
            // Calcula idade formatada a partir da DataNascimento ao mapear Domain para DTO
            CreateMap<Pessoa, PessoaDto>()
                .ForMember(dest => dest.Idade, opt => opt.MapFrom(src => DateHelper.CalcularIdadeFormatada(src.DataNascimento)));
            CreateMap<Categoria, CategoriaDto>();
            CreateMap<Transacao, TransacaoDto>();
        }
    }
}
