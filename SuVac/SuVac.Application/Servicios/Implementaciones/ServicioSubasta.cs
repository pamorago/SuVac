using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Servicios.Interfaces;
using SuVac.Infraestructure.Repositorio.Interfaces;

namespace SuVac.Application.Servicios.Implementaciones;

public class ServicioSubasta : IServicioSubasta
{
    private readonly IRepositorioSubasta _repositorioSubasta;
    private readonly IMapper _mapper;

    public ServicioSubasta(IRepositorioSubasta repositorioSubasta, IMapper mapper)
    {
        _repositorioSubasta = repositorioSubasta;
        _mapper = mapper;
    }

    public async Task<ICollection<SubastaListadoDTO>> ListarActivasAsync()
    {
        var lista = await _repositorioSubasta.ListarActivasAsync();
        return _mapper.Map<ICollection<SubastaListadoDTO>>(lista);
    }

    public async Task<ICollection<SubastaListadoDTO>> ListarFinalizadasAsync()
    {
        var lista = await _repositorioSubasta.ListarFinalizadasAsync();
        return _mapper.Map<ICollection<SubastaListadoDTO>>(lista);
    }

    public async Task<SubastaDetalleDTO?> BuscarPorIdAsync(int id)
    {
        var entidad = await _repositorioSubasta.BuscarPorIdAsync(id);
        if (entidad is null) return null;
        return _mapper.Map<SubastaDetalleDTO>(entidad);
    }
}
