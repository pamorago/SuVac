using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceSubasta : IServiceSubasta
{
    private readonly IRepositorySubasta _repository;
    private readonly IMapper _mapper;

    public ServiceSubasta(IRepositorySubasta repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SubastaDTO>> GetAll()
    {
        var subastas = await _repository.GetAll();
        return _mapper.Map<IEnumerable<SubastaDTO>>(subastas);
    }

    public async Task<SubastaDTO> GetById(int id)
    {
        var subasta = await _repository.GetById(id);
        return _mapper.Map<SubastaDTO>(subasta);
    }

    public async Task<bool> Create(SubastaDTO dto)
    {
        var subasta = _mapper.Map<Subasta>(dto);
        return await _repository.Create(subasta);
    }

    public async Task<bool> Update(SubastaDTO dto)
    {
        var subasta = _mapper.Map<Subasta>(dto);
        return await _repository.Update(subasta);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
