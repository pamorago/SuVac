using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class SubastaProfile : Profile
{
    public SubastaProfile()
    {
        CreateMap<Subasta, SubastaDTO>().ReverseMap();
    }
}
