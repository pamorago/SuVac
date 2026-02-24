using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceGanado : IServiceGanado
{
    private readonly IRepositoryGanado _repository;
    private readonly IMapper _mapper;

    public ServiceGanado(IRepositoryGanado repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GanadoDTO>> GetAll()
    {
        var ganados = await _repository.GetAll();
        return _mapper.Map<IEnumerable<GanadoDTO>>(ganados);
    }

    public async Task<GanadoDTO> GetById(int id)
    {
        var ganado = await _repository.GetById(id);
        return _mapper.Map<GanadoDTO>(ganado);
    }

    public async Task<bool> Create(GanadoDTO dto)
    {
        var ganado = _mapper.Map<Ganado>(dto);
        return await _repository.Create(ganado);
    }

    public async Task<bool> Update(GanadoDTO dto)
    {
        var ganado = _mapper.Map<Ganado>(dto);
        return await _repository.Update(ganado);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
