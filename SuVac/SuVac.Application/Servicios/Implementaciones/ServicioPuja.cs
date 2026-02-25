using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Servicios.Interfaces;
using SuVac.Infraestructure.Repositorio.Interfaces;

namespace SuVac.Application.Servicios.Implementaciones;

public class ServicioPuja : IServicioPuja
{
    private readonly IRepositorioPuja _repositorioPuja;
    private readonly IMapper _mapper;

    public ServicioPuja(IRepositorioPuja repositorioPuja, IMapper mapper)
    {
        _repositorioPuja = repositorioPuja;
        _mapper = mapper;
    }

    public async Task<ICollection<PujaDTO>> ListarPorSubastaAsync(int subastaId)
    {
        var lista = await _repositorioPuja.ListarPorSubastaAsync(subastaId);
        return _mapper.Map<ICollection<PujaDTO>>(lista);
    }
}
