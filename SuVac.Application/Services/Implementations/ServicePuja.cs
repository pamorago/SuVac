using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServicePuja : IServicePuja
{
    private readonly IRepositoryPuja _repository;
    private readonly IRepositorySubasta _repositorySubasta;
    private readonly IMapper _mapper;

    public ServicePuja(IRepositoryPuja repository, IRepositorySubasta repositorySubasta, IMapper mapper)
    {
        _repository = repository;
        _repositorySubasta = repositorySubasta;
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

    public async Task<IEnumerable<PujaDTO>> GetBySubasta(int subastaId)
    {
        var pujas = await _repository.GetBySubasta(subastaId);
        return pujas.Select(p => new PujaDTO
        {
            PujaId = p.PujaId,
            SubastaId = p.SubastaId,
            UsuarioId = p.UsuarioId,
            NombreUsuario = p.IdUsuarioNavigation.NombreCompleto,
            Monto = p.Monto,
            FechaHora = p.FechaHora
        });
    }

    public async Task<PujaDTO?> GetPujaMasAlta(int subastaId)
    {
        var puja = await _repository.GetPujaMasAlta(subastaId);
        if (puja is null) return null;

        return new PujaDTO
        {
            PujaId = puja.PujaId,
            SubastaId = puja.SubastaId,
            UsuarioId = puja.UsuarioId,
            NombreUsuario = puja.IdUsuarioNavigation.NombreCompleto,
            Monto = puja.Monto,
            FechaHora = puja.FechaHora
        };
    }

    public async Task<(bool ok, string mensaje, PujaDTO? puja)> RegistrarPujaValidada(
        int subastaId, int usuarioId, decimal monto)
    {
        // 1. Verificar que la subasta existe y está activa
        var subasta = await _repositorySubasta.GetByIdFull(subastaId);
        if (subasta is null)
            return (false, "La subasta no existe.", null);

        if (subasta.IdEstadoSubastaNavigation.Nombre != "Activa")
            return (false, "La subasta no está activa. No se pueden registrar pujas.", null);

        // 2. El usuario no puede ser el vendedor
        if (subasta.UsuarioCreadorId == usuarioId)
            return (false, "El vendedor no puede realizar pujas en su propia subasta.", null);

        // 3. Obtener la puja más alta actual
        var pujaMasAlta = await _repository.GetPujaMasAlta(subastaId);
        var montoReferencia = pujaMasAlta?.Monto ?? subasta.PrecioBase;

        // 4. Validar que el monto supera la referencia + incremento mínimo
        var montoMinimo = montoReferencia + subasta.IncrementoMinimo;
        if (monto < montoMinimo)
        {
            var descripcionReferencia = pujaMasAlta is null ? "el precio base" : "la puja actual";
            return (false,
                $"El monto debe ser al menos ₡{montoMinimo:N2} " +
                $"({descripcionReferencia} ₡{montoReferencia:N2} + incremento mínimo ₡{subasta.IncrementoMinimo:N2}).",
                null);
        }

        // 5. Registrar la puja
        var nuevaPuja = new Puja
        {
            SubastaId = subastaId,
            UsuarioId = usuarioId,
            Monto = monto,
            FechaHora = DateTime.Now
        };

        var ok = await _repository.Create(nuevaPuja);
        if (!ok)
            return (false, "Error al guardar la puja. Inténtelo de nuevo.", null);

        // 6. Cargar la puja con el nombre del usuario para el broadcast
        var pujaGuardada = await _repository.GetById(nuevaPuja.PujaId);
        var dto = new PujaDTO
        {
            PujaId = pujaGuardada!.PujaId,
            SubastaId = pujaGuardada.SubastaId,
            UsuarioId = pujaGuardada.UsuarioId,
            NombreUsuario = pujaGuardada.IdUsuarioNavigation.NombreCompleto,
            Monto = pujaGuardada.Monto,
            FechaHora = pujaGuardada.FechaHora
        };

        return (true, "Puja registrada exitosamente.", dto);
    }
}
