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

    // â”€â”€ Avance 2: mapeo manual para DTOs con campos calculados â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    public async Task<IEnumerable<SubastaListadoDTO>> GetActivas()
    {
        var subastas = await _repository.GetActivas();
        return subastas.Select(s => new SubastaListadoDTO
        {
            SubastaId = s.SubastaId,
            NombreGanado = s.IdGanadoNavigation.Nombre,
            ImagenGanado = s.IdGanadoNavigation.ImagenesGanado.FirstOrDefault()?.UrlImagen,
            FechaInicio = s.FechaInicio,
            FechaFin = s.FechaFin,
            PrecioBase = s.PrecioBase,
            IncrementoMinimo = s.IncrementoMinimo,
            EstadoSubasta = s.IdEstadoSubastaNavigation.Nombre,
            CantidadPujas = s.Pujas.Count
        }).ToList();
    }

    public async Task<IEnumerable<SubastaListadoDTO>> GetFinalizadas()
    {
        var subastas = await _repository.GetFinalizadas();
        return subastas.Select(s => new SubastaListadoDTO
        {
            SubastaId = s.SubastaId,
            NombreGanado = s.IdGanadoNavigation.Nombre,
            ImagenGanado = s.IdGanadoNavigation.ImagenesGanado.FirstOrDefault()?.UrlImagen,
            FechaInicio = s.FechaInicio,
            FechaFin = s.FechaFin,
            PrecioBase = s.PrecioBase,
            IncrementoMinimo = s.IncrementoMinimo,
            EstadoSubasta = s.IdEstadoSubastaNavigation.Nombre,
            CantidadPujas = s.Pujas.Count
        }).ToList();
    }

    public async Task<SubastaDetalleDTO?> GetDetalle(int id)
    {
        var s = await _repository.GetByIdFull(id);
        if (s is null) return null;

        return new SubastaDetalleDTO
        {
            SubastaId = s.SubastaId,
            NombreGanado = s.IdGanadoNavigation.Nombre,
            TipoGanado = s.IdGanadoNavigation.IdTipoGanadoNavigation.Nombre,
            EstadoGanado = s.IdGanadoNavigation.IdEstadoGanadoNavigation.Nombre,
            Categorias = s.IdGanadoNavigation.GanadoCategorias
                                 .Select(gc => gc.IdCategoriaNavigation.Nombre)
                                 .ToList(),
            ImagenesGanado = s.IdGanadoNavigation.ImagenesGanado
                                 .Select(i => i.UrlImagen)
                                 .ToList(),
            FechaInicio = s.FechaInicio,
            FechaFin = s.FechaFin,
            PrecioBase = s.PrecioBase,
            IncrementoMinimo = s.IncrementoMinimo,
            EstadoSubasta = s.IdEstadoSubastaNavigation.Nombre,
            TotalPujas = s.Pujas.Count,
            NombreCreador = s.IdUsuarioCreadorNavigation.NombreCompleto
        };
    }
}
