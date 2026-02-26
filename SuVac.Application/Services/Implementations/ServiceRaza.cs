using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceRaza : IServiceRaza
{
    private readonly IRepositoryRaza _repository;
    private readonly IMapper _mapper;

    public ServiceRaza(IRepositoryRaza repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RazaDTO>> GetAll()
    {
        var razas = await _repository.GetAll();
        return _mapper.Map<IEnumerable<RazaDTO>>(razas);
    }

    public async Task<RazaDTO> GetById(int id)
    {
        var raza = await _repository.GetById(id);
        return _mapper.Map<RazaDTO>(raza);
    }

    public async Task<bool> Create(RazaDTO dto)
    {
        var raza = _mapper.Map<Raza>(dto);
        return await _repository.Create(raza);
    }

    public async Task<bool> Update(RazaDTO dto)
    {
        var raza = _mapper.Map<Raza>(dto);
        return await _repository.Update(raza);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
