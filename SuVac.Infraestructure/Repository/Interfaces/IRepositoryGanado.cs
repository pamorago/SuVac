using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryGanado
{
    Task<IEnumerable<Ganado>> GetAll();
    Task<Ganado?> GetById(int id);
    Task<bool> Create(Ganado entity);
    Task<bool> Update(Ganado entity);
    /// <summary>Actualiza campos escalares + reemplaza colección de imágenes y categorías.</summary>
    Task<bool> UpdateFull(Ganado entity, List<int> categoriasIds, List<string> imagenesUrls);
    Task<bool> Delete(int id);
    /// <summary>Cambia EstadoGanadoId al valor indicado (1=Activo, 2=Inactivo).</summary>
    Task<bool> ToggleEstado(int id, int estadoId);
}
