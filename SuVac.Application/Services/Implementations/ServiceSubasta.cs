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
            DescripcionGanado = s.IdGanadoNavigation.Descripcion,
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
            NombreCreador = s.IdUsuarioCreadorNavigation.NombreCompleto,
            UsuarioCreadorId = s.UsuarioCreadorId
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

    public async Task<(bool ok, string mensaje, int subastaId)> CreateValidado(SubastaDTO dto)
    {
        // Fecha cierre debe ser posterior a fecha inicio
        if (dto.FechaFin <= dto.FechaInicio)
            return (false, "La fecha de cierre debe ser posterior a la fecha de inicio.", 0);

        // Precio base > 0 (complementa DataAnnotations)
        if (dto.PrecioBase <= 0)
            return (false, "El precio base debe ser mayor a ₡0.", 0);

        if (dto.IncrementoMinimo <= 0)
            return (false, "El incremento mínimo debe ser mayor a ₡0.", 0);

        // El ganado debe estar en estado Activo
        var ganadoActivo = await _repository.GanadoEstaActivo(dto.GanadoId);
        if (!ganadoActivo)
            return (false, "El ganado seleccionado no está activo y no puede ser subastado.", 0);

        // El ganado no puede tener otra subasta activa/programada/borrador
        if (await _repository.GanadoTieneSubastaActiva(dto.GanadoId))
            return (false, "El ganado seleccionado ya tiene una subasta activa o programada.", 0);

        // Estado inicial = Borrador
        var estadoBorrador = await _repository.GetEstadoIdByNombre("Borrador");
        if (estadoBorrador == null)
            return (false, "Estado 'Borrador' no configurado en el sistema. Ejecute el script SQL.", 0);

        dto.EstadoSubastaId = estadoBorrador.Value;

        var subasta = _mapper.Map<Subasta>(dto);
        var ok = await _repository.Create(subasta);
        return (ok, ok ? "Subasta creada correctamente como borrador." : "Error al guardar la subasta.", subasta.SubastaId);
    }

    public async Task<(bool ok, string mensaje)> UpdateValidado(SubastaDTO dto)
    {
        var subasta = await _repository.GetById(dto.SubastaId);
        if (subasta == null)
            return (false, "Subasta no encontrada.");

        // No puede editarse si ya tiene pujas
        if (await _repository.TienePujas(dto.SubastaId))
            return (false, "No se puede editar: la subasta ya tiene pujas registradas.");

        // No puede editarse si está Finalizada o Cancelada
        var idFinalizada = await _repository.GetEstadoIdByNombre("Finalizada");
        var idCancelada = await _repository.GetEstadoIdByNombre("Cancelada");
        if (subasta.EstadoSubastaId == idFinalizada || subasta.EstadoSubastaId == idCancelada)
            return (false, "No se puede editar una subasta Finalizada o Cancelada.");

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

        if (subasta.FechaFin <= DateTime.Now)
            return (false, "No se puede publicar: la fecha de cierre debe ser posterior a la fecha y hora actual.");

        // Si FechaInicio ya pasó o es ahora → activar directamente; si es futura → programar
        bool inicioYaPaso = subasta.FechaInicio <= DateTime.Now;
        string estadoDestino = inicioYaPaso ? "Activa" : "Programada";

        var idDestino = await _repository.GetEstadoIdByNombre(estadoDestino);
        if (idDestino == null)
            return (false, $"Estado '{estadoDestino}' no configurado en el sistema.");

        var ok = await _repository.CambiarEstado(subastaId, idDestino.Value);
        string msg = ok
            ? (inicioYaPaso
                ? "Subasta activada. Ahora está Activa y disponible para pujas."
                : "Subasta publicada correctamente. Ahora está Programada.")
            : "Error al publicar la subasta.";
        return (ok, msg);
    }

    public async Task<(bool ok, string mensaje)> Cancelar(int subastaId)
    {
        var subasta = await _repository.GetById(subastaId);
        if (subasta == null)
            return (false, "Subasta no encontrada.");

        // No se puede cancelar si ya está Finalizada o Cancelada
        var idFinalizada = await _repository.GetEstadoIdByNombre("Finalizada");
        var idCancelada = await _repository.GetEstadoIdByNombre("Cancelada");

        if (subasta.EstadoSubastaId == idFinalizada)
            return (false, "No se puede cancelar: la subasta ya está Finalizada.");
        if (subasta.EstadoSubastaId == idCancelada)
            return (false, "La subasta ya está Cancelada.");

        // No se puede cancelar si tiene pujas (sin importar el estado)
        if (await _repository.TienePujas(subastaId))
            return (false, "No se puede cancelar: la subasta ya tiene pujas registradas.");

        var idCanceladaDestino = await _repository.GetEstadoIdByNombre("Cancelada");
        if (idCanceladaDestino == null)
            return (false, "Estado 'Cancelada' no configurado en el sistema.");

        var ok = await _repository.CambiarEstado(subastaId, idCanceladaDestino.Value);
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

    public async Task ActualizarEstadosAsync()
    {
        await _repository.ActualizarEstadosAsync();
    }

    public async Task<IEnumerable<int>> GetIdsActivasParaFinalizarAsync()
    {
        return await _repository.GetIdsActivasParaFinalizarAsync();
    }
}

