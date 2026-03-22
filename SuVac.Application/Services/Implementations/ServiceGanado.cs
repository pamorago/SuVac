using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceGanado : IServiceGanado
{
    private readonly IRepositoryGanado _repository;
    private readonly IMapper _mapper;

    public ServiceGanado(IRepositoryGanado repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GanadoDTO>> GetAll()
    {
        var ganados = await _repository.GetAll();
        var dtos = _mapper.Map<IEnumerable<GanadoDTO>>(ganados).ToList();

        // Poblar historial de subastas para poder condicionar acciones en el listado
        var ganadosList = ganados.ToList();
        for (int i = 0; i < dtos.Count; i++)
        {
            var subastas = ganadosList[i].Subastas;
            if (subastas != null)
            {
                dtos[i].SubastasParticipacion = subastas
                    .OrderByDescending(s => s.FechaInicio)
                    .Select(s => new SubastaResumenDTO
                    {
                        SubastaId = s.SubastaId,
                        FechaInicio = s.FechaInicio,
                        FechaFin = s.FechaFin,
                        EstadoSubasta = s.IdEstadoSubastaNavigation?.Nombre ?? "-"
                    })
                    .ToList();
            }
        }
        return dtos;
    }

    public async Task<GanadoDTO> GetById(int id)
    {
        var ganado = await _repository.GetById(id);
        var dto = _mapper.Map<GanadoDTO>(ganado);

        // Calcular historial de subastas via LINQ (campo calculado, no almacenado)
        if (ganado?.Subastas != null)
        {
            dto.SubastasParticipacion = ganado.Subastas
                .OrderByDescending(s => s.FechaInicio)
                .Select(s => new SubastaResumenDTO
                {
                    SubastaId = s.SubastaId,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    EstadoSubasta = s.IdEstadoSubastaNavigation?.Nombre ?? "-"
                })
                .ToList();
        }

        return dto;
    }

    public async Task<bool> Create(GanadoDTO dto)
    {
        var ganado = _mapper.Map<Ganado>(dto);
        ganado.FechaRegistro = DateTime.Now;
        ganado.EstadoGanadoId = 1; // Activo por defecto al crear

        // Agregar imágenes
        foreach (var imgDto in dto.ImagenesGanado?.Where(i => !string.IsNullOrWhiteSpace(i.UrlImagen)) ?? [])
            ganado.ImagenesGanado.Add(new ImagenGanado { UrlImagen = imgDto.UrlImagen });

        // Agregar categorías
        foreach (var catId in dto.CategoriasIds ?? [])
            ganado.GanadoCategorias.Add(new GanadoCategoria { CategoriaId = catId });

        return await _repository.Create(ganado);
    }

    public async Task<bool> Update(GanadoDTO dto)
    {
        var ganado = _mapper.Map<Ganado>(dto);
        var imagenesUrls = dto.ImagenesGanado
            ?.Select(i => i.UrlImagen)
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .ToList() ?? [];
        return await _repository.UpdateFull(ganado, dto.CategoriasIds ?? [], imagenesUrls);
    }

    /// <summary>Eliminación lógica: establece EstadoGanadoId = 2 (Inactivo).</summary>
    public async Task<bool> Delete(int id)
        => await _repository.ToggleEstado(id, 2);

    public async Task<bool> ToggleEstado(int id, int estadoId)
        => await _repository.ToggleEstado(id, estadoId);
}
