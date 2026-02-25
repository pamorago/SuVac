using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceTipoGanado : IServiceTipoGanado
{
    private readonly IRepositoryTipoGanado _repository;
    private readonly IMapper _mapper;

    public ServiceTipoGanado(IRepositoryTipoGanado repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TipoGanadoDTO>> GetAll()
    {
        var tiposGanado = await _repository.GetAll();
        return _mapper.Map<IEnumerable<TipoGanadoDTO>>(tiposGanado);
    }

    public async Task<TipoGanadoDTO> GetById(int id)
    {
        var tipoGanado = await _repository.GetById(id);
        return _mapper.Map<TipoGanadoDTO>(tipoGanado);
    }

    public async Task<bool> Create(TipoGanadoDTO dto)
    {
        var tipoGanado = _mapper.Map<TipoGanado>(dto);
        return await _repository.Create(tipoGanado);
    }

    public async Task<bool> Update(TipoGanadoDTO dto)
    {
        var tipoGanado = _mapper.Map<TipoGanado>(dto);
        return await _repository.Update(tipoGanado);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
