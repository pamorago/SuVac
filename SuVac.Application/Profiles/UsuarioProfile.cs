using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDTO>()
            .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.IdRolNavigation != null ? src.IdRolNavigation.Nombre : null))
            .ForMember(dest => dest.NombreEstado, opt => opt.MapFrom(src => src.IdEstadoNavigation != null ? src.IdEstadoNavigation.Nombre : null))
            .ForMember(dest => dest.CantidadSubastasCreadas, opt => opt.MapFrom(src => src.Subastas != null ? src.Subastas.Count : 0))
            .ForMember(dest => dest.CantidadPujasRealizadas, opt => opt.MapFrom(src => src.Pujas != null ? src.Pujas.Count : 0));

        CreateMap<UsuarioDTO, Usuario>()
            .ForMember(dest => dest.IdRolNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdEstadoNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.Subastas, opt => opt.Ignore())
            .ForMember(dest => dest.Pujas, opt => opt.Ignore())
            .ForMember(dest => dest.Ganados, opt => opt.Ignore())
            .ForMember(dest => dest.ResultadosSubasta, opt => opt.Ignore())
            .ForMember(dest => dest.Pagos, opt => opt.Ignore());
    }
}

