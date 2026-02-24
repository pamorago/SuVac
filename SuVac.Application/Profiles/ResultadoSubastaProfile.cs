using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class ResultadoSubastaProfile : Profile
{
    public ResultadoSubastaProfile()
    {
        CreateMap<ResultadoSubasta, ResultadoSubastaDTO>().ReverseMap();
    }
}
