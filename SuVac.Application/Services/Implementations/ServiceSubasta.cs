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

    public async Task<IEnumerable<SubastaListadoDTO>> GetActivas()
    {
        var subastas = await _repository.GetActivas();
        var listado = new List<SubastaListadoDTO>();

        foreach (var s in subastas)
        {
            // Campo calculado: cantidad de pujas mediante LINQ CountAsync
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
            // Campo calculado: cantidad de pujas mediante LINQ CountAsync
            var cantPujas = await _repository.CountPujasAsync(s.SubastaId);
            listado.Add(ToListadoDTO(s, cantPujas));
        }

        return listado;
    }

    public async Task<SubastaDetalleDTO?> GetDetalle(int id)
    {
        var s = await _repository.GetByIdFull(id);
        if (s is null) return null;

        // Campo calculado: total de pujas mediante LINQ CountAsync (no almacenado en BD)
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

    /// <summary>
    /// Mapea una Subasta a SubastaListadoDTO. CantidadPujas se recibe como parámetro
    /// ya que fue calculado mediante LINQ CountAsync.
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
        CantidadPujas = cantidadPujas
    };
}
