using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServicePago : IServicePago
{
    private readonly IRepositoryPago _repository;
    private readonly IMapper _mapper;

    public ServicePago(IRepositoryPago repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PagoDTO>> GetAll()
    {
        var pagos = await _repository.GetAll();
        return _mapper.Map<IEnumerable<PagoDTO>>(pagos);
    }

    public async Task<PagoDTO> GetById(int id)
    {
        var pago = await _repository.GetById(id);
        return _mapper.Map<PagoDTO>(pago);
    }

    public async Task<bool> Create(PagoDTO dto)
    {
        var pago = _mapper.Map<Pago>(dto);
        return await _repository.Create(pago);
    }

    public async Task<bool> Update(PagoDTO dto)
    {
        var pago = _mapper.Map<Pago>(dto);
        return await _repository.Update(pago);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }
}
