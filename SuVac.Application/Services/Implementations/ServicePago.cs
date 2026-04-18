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

    public async Task<PagoDTO?> GetById(int id)
    {
        var pago = await _repository.GetById(id);
        return pago is null ? null : _mapper.Map<PagoDTO>(pago);
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

    public async Task<IEnumerable<PagoDTO>> GetAllConDetalle()
    {
        var pagos = await _repository.GetAllConDetalle();
        return _mapper.Map<IEnumerable<PagoDTO>>(pagos);
    }

    public async Task<PagoDTO?> GetByIdConDetalle(int id)
    {
        var pago = await _repository.GetByIdConDetalle(id);
        return pago is null ? null : _mapper.Map<PagoDTO>(pago);
    }

    public async Task<PagoDTO?> GetBySubastaId(int subastaId)
    {
        var pago = await _repository.GetBySubastaId(subastaId);
        return pago is null ? null : _mapper.Map<PagoDTO>(pago);
    }

    public async Task<(bool ok, string mensaje)> RegistrarPagoGanador(
        int subastaId, int usuarioGanadorId, decimal montoFinal)
    {
        // Idempotente: no crear duplicados
        if (await _repository.ExistePagoParaSubasta(subastaId))
            return (false, "Ya existe un pago registrado para esta subasta.");

        var idPendiente = await _repository.GetEstadoIdByNombre("Pendiente");
        if (idPendiente is null)
            return (false, "No se encontró el estado de pago 'Pendiente'.");

        var pago = new Pago
        {
            SubastaId = subastaId,
            UsuarioId = usuarioGanadorId,
            Monto = montoFinal,
            EstadoPagoId = idPendiente.Value,
            FechaPago = DateTime.Now
        };

        var ok = await _repository.Create(pago);
        return ok
            ? (true, "Pago registrado correctamente en estado Pendiente.")
            : (false, "Error al registrar el pago en la base de datos.");
    }

    public async Task<(bool ok, string mensaje)> ConfirmarPago(int pagoId)
    {
        var pago = await _repository.GetByIdConDetalle(pagoId);
        if (pago is null)
            return (false, "No se encontró el pago.");

        if (pago.IdEstadoPagoNavigation?.Nombre == "Confirmado")
            return (false, "El pago ya está confirmado.");

        var ok = await _repository.ConfirmarPago(pagoId);
        return ok
            ? (true, "Pago confirmado correctamente.")
            : (false, "Error al confirmar el pago.");
    }
}
