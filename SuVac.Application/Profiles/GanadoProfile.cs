using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class GanadoProfile : Profile
{
    public GanadoProfile()
    {
        CreateMap<Ganado, GanadoDTO>()
            .ForMember(dest => dest.NombreTipoGanado,
                opt => opt.MapFrom(src => src.IdTipoGanadoNavigation != null ? src.IdTipoGanadoNavigation.Nombre : null))
            .ForMember(dest => dest.NombreRaza,
                opt => opt.MapFrom(src => src.IdRazaNavigation != null ? src.IdRazaNavigation.Nombre : null))
            .ForMember(dest => dest.NombreSexo,
                opt => opt.MapFrom(src => src.IdSexoNavigation != null ? src.IdSexoNavigation.Nombre : null))
            .ForMember(dest => dest.NombreVendedor,
                opt => opt.MapFrom(src => src.IdUsuarioVendedorNavigation != null ? src.IdUsuarioVendedorNavigation.NombreCompleto : null))
            .ReverseMap();
    }
}
