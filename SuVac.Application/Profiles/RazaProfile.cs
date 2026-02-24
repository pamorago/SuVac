using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class RazaProfile : Profile
{
    public RazaProfile()
    {
        CreateMap<Raza, RazaDTO>().ReverseMap();
    }
}
