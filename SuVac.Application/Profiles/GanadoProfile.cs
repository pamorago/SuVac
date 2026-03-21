using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class GanadoProfile : Profile
{
    public GanadoProfile()
    {
        // Ganado → GanadoDTO (for display)
        CreateMap<Ganado, GanadoDTO>()
            .ForMember(dest => dest.NombreTipoGanado,
                opt => opt.MapFrom(src => src.IdTipoGanadoNavigation != null ? src.IdTipoGanadoNavigation.Nombre : null))
            .ForMember(dest => dest.NombreRaza,
                opt => opt.MapFrom(src => src.IdRazaNavigation != null ? src.IdRazaNavigation.Nombre : null))
            .ForMember(dest => dest.NombreSexo,
                opt => opt.MapFrom(src => src.IdSexoNavigation != null ? src.IdSexoNavigation.Nombre : null))
            .ForMember(dest => dest.NombreEstadoGanado,
                opt => opt.MapFrom(src => src.IdEstadoGanadoNavigation != null ? src.IdEstadoGanadoNavigation.Nombre : null))
            .ForMember(dest => dest.NombreVendedor,
                opt => opt.MapFrom(src => src.IdUsuarioVendedorNavigation != null ? src.IdUsuarioVendedorNavigation.NombreCompleto : null))
            .ForMember(dest => dest.Categorias,
                opt => opt.MapFrom(src => src.GanadoCategorias != null
                    ? src.GanadoCategorias
                        .Where(gc => gc.IdCategoriaNavigation != null)
                        .Select(gc => gc.IdCategoriaNavigation.Nombre)
                        .ToList()
                    : null))
            .ForMember(dest => dest.CategoriasIds,
                opt => opt.MapFrom(src => src.GanadoCategorias != null
                    ? src.GanadoCategorias.Select(gc => gc.CategoriaId).ToList()
                    : null))
            .ForMember(dest => dest.ImagenesGanado,
                opt => opt.MapFrom(src => src.ImagenesGanado))
            .ForMember(dest => dest.SubastasParticipacion, opt => opt.Ignore());

        // GanadoDTO → Ganado (for persistence — collections handled manually in service)
        CreateMap<GanadoDTO, Ganado>()
            .ForMember(dest => dest.GanadoCategorias, opt => opt.Ignore())
            .ForMember(dest => dest.Subastas, opt => opt.Ignore())
            .ForMember(dest => dest.ImagenesGanado, opt => opt.Ignore())
            .ForMember(dest => dest.IdTipoGanadoNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdRazaNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdSexoNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdEstadoGanadoNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdUsuarioVendedorNavigation, opt => opt.Ignore());
    }
}
