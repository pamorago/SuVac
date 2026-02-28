using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class ImagenGanadoProfile : Profile
{
    public ImagenGanadoProfile()
    {
        CreateMap<ImagenGanado, ImagenGanadoDTO>().ReverseMap();
    }
}
