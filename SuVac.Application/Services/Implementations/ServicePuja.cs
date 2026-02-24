using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServicePuja : IServicePuja
{
    private readonly IRepositoryPuja _repository;
    private readonly IMapper _mapper;

    public ServicePuja(IRepositoryPuja repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PujaDTO>> GetAll()
    {
        var pujas = await _repository.GetAll();
        return _mapper.Map<IEnumerable<PujaDTO>>(pujas);
    }

    public async Task<PujaDTO> GetById(int id)
    {
        var puja = await _repository.GetById(id);
        return _mapper.Map<PujaDTO>(puja);
    }

    public async Task<bool> Create(PujaDTO dto)
    {
        var puja = _mapper.Map<Puja>(dto);
        return await _repository.Create(puja);
    }

    public async Task<bool> Update(PujaDTO dto)
    {
        var puja = _mapper.Map<Puja>(dto);
        return await _repository.Update(puja);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
