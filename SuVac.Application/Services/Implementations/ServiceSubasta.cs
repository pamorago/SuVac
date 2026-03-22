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

    public async Task<SubastaDTO?> GetById(int id)
    {
        // Usa GetByIdFull para cargar navegaciones y poder poblar campos de visualización
        var subasta = await _repository.GetByIdFull(id);
        if (subasta is null) return null;

        var dto = _mapper.Map<SubastaDTO>(subasta);
        dto.NombreGanado = subasta.IdGanadoNavigation?.Nombre;
        dto.NombreCreador = subasta.IdUsuarioCreadorNavigation?.NombreCompleto;
        dto.NombreEstadoSubasta = subasta.IdEstadoSubastaNavigation?.Nombre;
        return dto;
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

    public async Task<IEnumerable<SubastaListadoDTO>> GetActivas()
    {
        var subastas = await _repository.GetActivas();
        var listado = new List<SubastaListadoDTO>();

        foreach (var s in subastas)
        {
            var cantPujas = await _repository.CountPujasAsync(s.SubastaId);
            listado.Add(ToListadoDTO(s, cantPujas));
        }

        return listado;
    }

    public async Task<IEnumerable<SubastaListadoDTO>> GetFinalizadas()
    {
        var subastas = await _repository.GetFinalizadas();
        var listado = new List<SubastaListadoDTO>();

        foreach (var s in subastas)
        {
            var cantPujas = await _repository.CountPujasAsync(s.SubastaId);
            listado.Add(ToListadoDTO(s, cantPujas));
        }

        return listado;
    }

    public async Task<SubastaDetalleDTO?> GetDetalle(int id)
    {
        var s = await _repository.GetByIdFull(id);
        if (s is null) return null;

        var totalPujas = await _repository.CountPujasAsync(id);

        return new SubastaDetalleDTO
        {
            SubastaId = s.SubastaId,
            GanadoId = s.GanadoId,
            NombreGanado = s.IdGanadoNavigation.Nombre,
            TipoGanado = s.IdGanadoNavigation.IdTipoGanadoNavigation?.Nombre ?? "-",
            EstadoGanado = s.IdGanadoNavigation.IdEstadoGanadoNavigation?.Nombre ?? "-",
            CertificadoSalud = s.IdGanadoNavigation.CertificadoSalud,
            Categorias = s.IdGanadoNavigation.GanadoCategorias
                                 .Select(gc => gc.IdCategoriaNavigation.Nombre).ToList(),
            ImagenesGanado = s.IdGanadoNavigation.ImagenesGanado
                                 .Select(i => i.UrlImagen).ToList(),
            FechaInicio = s.FechaInicio,
            FechaFin = s.FechaFin,
            PrecioBase = s.PrecioBase,
            IncrementoMinimo = s.IncrementoMinimo,
            EstadoSubasta = s.IdEstadoSubastaNavigation.Nombre,
            TotalPujas = totalPujas,
            NombreCreador = s.IdUsuarioCreadorNavigation.NombreCompleto
        };
    }

    // ── Métodos administrativos ──────────────────────────────────────────────

    public async Task<IEnumerable<SubastaListadoDTO>> GetAllAdmin()
    {
        var subastas = await _repository.GetAllAdmin();
        var listado = new List<SubastaListadoDTO>();

        foreach (var s in subastas)
        {
            var cantPujas = await _repository.CountPujasAsync(s.SubastaId);
            listado.Add(ToListadoDTO(s, cantPujas));
        }

        return listado;
    }

    public async Task<IEnumerable<GanadoDTO>> GetGanadosActivos()
    {
        var ganados = await _repository.GetGanadosActivos();
        return ganados.Select(g => new GanadoDTO
        {
            GanadoId = g.GanadoId,
            Nombre = g.Nombre,
            UsuarioVendedorId = g.UsuarioVendedorId
        });
    }

    public async Task<(bool ok, string mensaje)> CreateValidado(SubastaDTO dto)
    {
        // Fecha cierre debe ser posterior a fecha inicio
        if (dto.FechaFin <= dto.FechaInicio)
            return (false, "La fecha de cierre debe ser posterior a la fecha de inicio.");

        // Precio base > 0 (complementa DataAnnotations)
        if (dto.PrecioBase <= 0)
            return (false, "El precio base debe ser mayor a ₡0.");

        if (dto.IncrementoMinimo <= 0)
            return (false, "El incremento mínimo debe ser mayor a ₡0.");

        // El ganado debe estar en estado Activo
        var ganadoActivo = await _repository.GanadoEstaActivo(dto.GanadoId);
        if (!ganadoActivo)
            return (false, "El ganado seleccionado no está activo y no puede ser subastado.");

        // El ganado no puede tener otra subasta activa/programada/borrador
        if (await _repository.GanadoTieneSubastaActiva(dto.GanadoId))
            return (false, "El ganado seleccionado ya tiene una subasta activa o programada.");

        // Estado inicial = Borrador
        var estadoBorrador = await _repository.GetEstadoIdByNombre("Borrador");
        if (estadoBorrador == null)
            return (false, "Estado 'Borrador' no configurado en el sistema. Ejecute el script SQL.");

        dto.EstadoSubastaId = estadoBorrador.Value;

        var subasta = _mapper.Map<Subasta>(dto);
        var ok = await _repository.Create(subasta);
        return (ok, ok ? "Subasta creada correctamente como borrador." : "Error al guardar la subasta.");
    }

    public async Task<(bool ok, string mensaje)> UpdateValidado(SubastaDTO dto)
    {
        var subasta = await _repository.GetById(dto.SubastaId);
        if (subasta == null)
            return (false, "Subasta no encontrada.");

        // No puede editarse si ya inició
        if (subasta.FechaInicio <= DateTime.Now)
            return (false, "No se puede editar: la subasta ya ha iniciado.");

        // No puede editarse si ya tiene pujas
        if (await _repository.TienePujas(dto.SubastaId))
            return (false, "No se puede editar: la subasta ya tiene pujas registradas.");

        // Fecha cierre > inicio
        if (dto.FechaFin <= dto.FechaInicio)
            return (false, "La fecha de cierre debe ser posterior a la fecha de inicio.");

        if (dto.PrecioBase <= 0)
            return (false, "El precio base debe ser mayor a ₡0.");

        if (dto.IncrementoMinimo <= 0)
            return (false, "El incremento mínimo debe ser mayor a ₡0.");

        // Solo se actualizan los campos permitidos — GanadoId, UsuarioCreadorId, EstadoSubastaId NO cambian
        subasta.FechaInicio = dto.FechaInicio;
        subasta.FechaFin = dto.FechaFin;
        subasta.PrecioBase = dto.PrecioBase;
        subasta.IncrementoMinimo = dto.IncrementoMinimo;

        var ok = await _repository.Update(subasta);
        return (ok, ok ? "Subasta actualizada correctamente." : "Error al actualizar la subasta.");
    }

    public async Task<(bool ok, string mensaje)> Publicar(int subastaId)
    {
        var subasta = await _repository.GetById(subastaId);
        if (subasta == null)
            return (false, "Subasta no encontrada.");

        var idBorrador = await _repository.GetEstadoIdByNombre("Borrador");
        if (subasta.EstadoSubastaId != idBorrador)
            return (false, "Solo se puede publicar una subasta que se encuentre en estado Borrador.");

        var idProgramada = await _repository.GetEstadoIdByNombre("Programada");
        if (idProgramada == null)
            return (false, "Estado 'Programada' no configurado en el sistema.");

        var ok = await _repository.CambiarEstado(subastaId, idProgramada.Value);
        return (ok, ok ? "Subasta publicada correctamente. Ahora está Programada." : "Error al publicar la subasta.");
    }

    public async Task<(bool ok, string mensaje)> Cancelar(int subastaId)
    {
        var subasta = await _repository.GetById(subastaId);
        if (subasta == null)
            return (false, "Subasta no encontrada.");

        var noHaIniciado = subasta.FechaInicio > DateTime.Now;
        var sinPujas = !await _repository.TienePujas(subastaId);

        if (!noHaIniciado && !sinPujas)
            return (false, "No se puede cancelar: la subasta ya inició y tiene pujas registradas.");

        var idCancelada = await _repository.GetEstadoIdByNombre("Cancelada");
        if (idCancelada == null)
            return (false, "Estado 'Cancelada' no configurado en el sistema.");

        var ok = await _repository.CambiarEstado(subastaId, idCancelada.Value);
        return (ok, ok ? "Subasta cancelada correctamente." : "Error al cancelar la subasta.");
    }

    /// <summary>
    /// Mapea una Subasta a SubastaListadoDTO.
    /// </summary>
    private static SubastaListadoDTO ToListadoDTO(Subasta s, int cantidadPujas) => new()
    {
        SubastaId = s.SubastaId,
        NombreGanado = s.IdGanadoNavigation.Nombre,
        ImagenGanado = s.IdGanadoNavigation.ImagenesGanado.FirstOrDefault()?.UrlImagen,
        FechaInicio = s.FechaInicio,
        FechaFin = s.FechaFin,
        PrecioBase = s.PrecioBase,
        IncrementoMinimo = s.IncrementoMinimo,
        EstadoSubasta = s.IdEstadoSubastaNavigation.Nombre,
        CantidadPujas = cantidadPujas,
        NombreCreador = s.IdUsuarioCreadorNavigation?.NombreCompleto ?? "-"
    };
}

