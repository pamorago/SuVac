using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Modelos;

namespace SuVac.Application.Perfiles;

public class SubastaPerfiles : Profile
{
    public SubastaPerfiles()
    {
        // Subasta → SubastaListadoDTO
        // CantidadPujas es un campo calculado: se mapea desde el conteo de Pujas
        CreateMap<Subasta, SubastaListadoDTO>()
            .ForMember(d => d.NombreGanado, o => o.MapFrom(s => s.GanadoNavigation.Nombre))
            .ForMember(d => d.ImagenGanado, o => o.MapFrom(s =>
                s.GanadoNavigation.Imagenes.Select(i => i.UrlImagen).FirstOrDefault()))
            .ForMember(d => d.EstadoSubasta, o => o.MapFrom(s => s.EstadoSubastaNavigation.Nombre))
            .ForMember(d => d.CantidadPujas, o => o.MapFrom(s => s.Pujas.Count));

        // Subasta → SubastaDetalleDTO
        CreateMap<Subasta, SubastaDetalleDTO>()
            .ForMember(d => d.NombreGanado, o => o.MapFrom(s => s.GanadoNavigation.Nombre))
            .ForMember(d => d.TipoGanado, o => o.MapFrom(s => s.GanadoNavigation.TipoGanadoNavigation.Nombre))
            .ForMember(d => d.EstadoGanado, o => o.MapFrom(s => s.GanadoNavigation.EstadoGanadoNavigation.Nombre))
            .ForMember(d => d.Categorias, o => o.MapFrom(s =>
                s.GanadoNavigation.Categorias.Select(c => c.Nombre).ToList()))
            .ForMember(d => d.ImagenesGanado, o => o.MapFrom(s =>
                s.GanadoNavigation.Imagenes.Select(i => i.UrlImagen).ToList()))
            .ForMember(d => d.EstadoSubasta, o => o.MapFrom(s => s.EstadoSubastaNavigation.Nombre))
            .ForMember(d => d.TotalPujas, o => o.MapFrom(s => s.Pujas.Count))
            .ForMember(d => d.NombreCreador, o => o.MapFrom(s => s.UsuarioCreadorNavigation.NombreCompleto));
    }
}
