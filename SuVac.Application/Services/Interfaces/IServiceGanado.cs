using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceGanado
{
    Task<IEnumerable<GanadoDTO>> GetAll();
    Task<GanadoDTO> GetById(int id);
    Task<bool> Create(GanadoDTO dto);
    Task<bool> Update(GanadoDTO dto);
    /// <summary>Eliminación lógica: cambia EstadoGanadoId a Inactivo (2). Solo llamar después de validar reglas de negocio en el controlador.</summary>
    Task<bool> Delete(int id);
    /// <summary>Cambia el EstadoGanadoId al valor indicado (1=Activo, 2=Inactivo).</summary>
    Task<bool> ToggleEstado(int id, int estadoId);
}
