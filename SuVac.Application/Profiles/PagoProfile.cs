using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Profiles;

public class PagoProfile : Profile
{
    public PagoProfile()
    {
        CreateMap<Pago, PagoDTO>().ReverseMap();
    }
}
