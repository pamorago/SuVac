using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class GanadoProfile : Profile
{
    public GanadoProfile()
    {
        CreateMap<Ganado, GanadoDTO>().ReverseMap();
    }
}
