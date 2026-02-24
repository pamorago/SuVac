using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class CategoriaProfile : Profile
{
    public CategoriaProfile()
    {
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
    }
}
