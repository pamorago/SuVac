using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceRol : IServiceRol
{
    private readonly IRepositoryRol _repository;
    private readonly IMapper _mapper;

    public ServiceRol(IRepositoryRol repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RolDTO>> GetAll()
    {
        var roles = await _repository.GetAll();
        return _mapper.Map<IEnumerable<RolDTO>>(roles);
    }

    public async Task<RolDTO> GetById(int id)
    {
        var rol = await _repository.GetById(id);
        return _mapper.Map<RolDTO>(rol);
    }

    public async Task<bool> Create(RolDTO dto)
    {
        var rol = _mapper.Map<Rol>(dto);
        return await _repository.Create(rol);
    }

    public async Task<bool> Update(RolDTO dto)
    {
        var rol = _mapper.Map<Rol>(dto);
        return await _repository.Update(rol);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
