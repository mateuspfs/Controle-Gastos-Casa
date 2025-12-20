using AutoMapper;
using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Domain.Entities;

namespace ControleGastosCasa.Application.Mapping
{
    public class DtoToDomainMapping : Profile
    {
        public DtoToDomainMapping()
        {
            CreateMap<PessoaDto, Pessoa>().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<CategoriaDto, Categoria>().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<TransacaoDto, Transacao>().ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
