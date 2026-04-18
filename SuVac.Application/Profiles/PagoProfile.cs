using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class PagoProfile : Profile
{
    public PagoProfile()
    {
        CreateMap<Pago, PagoDTO>()
            .ForMember(d => d.NombreGanado,
                o => o.MapFrom(s => s.IdSubastaNavigation != null
                    ? s.IdSubastaNavigation.IdGanadoNavigation != null
                        ? s.IdSubastaNavigation.IdGanadoNavigation.Nombre
                        : null
                    : null))
            .ForMember(d => d.NombreUsuario,
                o => o.MapFrom(s => s.IdUsuarioNavigation != null
                    ? s.IdUsuarioNavigation.NombreCompleto
                    : null))
            .ForMember(d => d.NombreEstadoPago,
                o => o.MapFrom(s => s.IdEstadoPagoNavigation != null
                    ? s.IdEstadoPagoNavigation.Nombre
                    : null));

        CreateMap<PagoDTO, Pago>()
            .ForMember(d => d.IdSubastaNavigation, o => o.Ignore())
            .ForMember(d => d.IdUsuarioNavigation, o => o.Ignore())
            .ForMember(d => d.IdEstadoPagoNavigation, o => o.Ignore());
    }
}
