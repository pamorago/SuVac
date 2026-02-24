using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDTO>().ReverseMap();
    }
}
