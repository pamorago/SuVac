using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class RolProfile : Profile
{
    public RolProfile()
    {
        CreateMap<Rol, RolDTO>().ReverseMap();
    }
}
