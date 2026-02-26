using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class PujaProfile : Profile
{
    public PujaProfile()
    {
        CreateMap<Puja, PujaDTO>()
            .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.IdUsuarioNavigation.NombreCompleto))
            .ReverseMap()
            .ForMember(dest => dest.IdUsuarioNavigation, opt => opt.Ignore());
    }
}
