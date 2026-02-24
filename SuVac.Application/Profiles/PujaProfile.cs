using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class PujaProfile : Profile
{
    public PujaProfile()
    {
        CreateMap<Puja, PujaDTO>().ReverseMap();
    }
}
