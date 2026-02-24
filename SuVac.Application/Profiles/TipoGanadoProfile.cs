using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class TipoGanadoProfile : Profile
{
    public TipoGanadoProfile()
    {
        CreateMap<TipoGanado, TipoGanadoDTO>().ReverseMap();
    }
}
