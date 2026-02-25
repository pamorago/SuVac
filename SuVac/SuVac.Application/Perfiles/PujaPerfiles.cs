using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Modelos;

namespace SuVac.Application.Perfiles;

public class PujaPerfiles : Profile
{
    public PujaPerfiles()
    {
        // Puja → PujaDTO
        CreateMap<Puja, PujaDTO>()
            .ForMember(d => d.NombreUsuario, o => o.MapFrom(p => p.UsuarioNavigation.NombreCompleto));
    }
}
