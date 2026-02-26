using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceResultadoSubasta : IServiceResultadoSubasta
{
    private readonly IRepositoryResultadoSubasta _repository;
    private readonly IMapper _mapper;

    public ServiceResultadoSubasta(IRepositoryResultadoSubasta repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResultadoSubastaDTO>> GetAll()
    {
        var resultados = await _repository.GetAll();
        return _mapper.Map<IEnumerable<ResultadoSubastaDTO>>(resultados);
    }

    public async Task<ResultadoSubastaDTO> GetById(int id)
    {
        var resultado = await _repository.GetById(id);
        return _mapper.Map<ResultadoSubastaDTO>(resultado);
    }

    public async Task<bool> Create(ResultadoSubastaDTO dto)
    {
        var resultado = _mapper.Map<ResultadoSubasta>(dto);
        return await _repository.Create(resultado);
    }

    public async Task<bool> Update(ResultadoSubastaDTO dto)
    {
        var resultado = _mapper.Map<ResultadoSubasta>(dto);
        return await _repository.Update(resultado);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }

    public async Task<ResultadoSubastaDTO> GetBySubastaId(int subastaId)
    {
        var resultado = await _repository.GetBySubastaId(subastaId);
        return _mapper.Map<ResultadoSubastaDTO>(resultado);
    }
}
