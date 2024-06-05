using AutoMapper;
using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Domain.Entities;

namespace Itau.Transfer.Infrastructure.Automaper
{
    public class TransferenciaMapper : Profile
    {
        public TransferenciaMapper()
        {
            CreateMap<TransferenciaDto, Transferencia>()
                .ForMember(dest => dest.TransferenciaContas, opt => opt.MapFrom(src => src.Conta))
                .ReverseMap()
                .ForMember(dest => dest.Conta, opt => opt.MapFrom(src => src.TransferenciaContas));

            CreateMap<TransferenciaContaDto, TransferenciaContas>().ReverseMap();
        }
    }
}