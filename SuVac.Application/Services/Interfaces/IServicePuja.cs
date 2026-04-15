using SuVac.Application.DTOs;

namespace SuVac.Application.Services.Interfaces;

public interface IServicePuja
{
    Task<IEnumerable<PujaDTO>> GetAll();
    Task<PujaDTO> GetById(int id);
    Task<bool> Create(PujaDTO dto);
    Task<bool> Update(PujaDTO dto);
    Task<bool> Delete(int id);

    Task<IEnumerable<PujaDTO>> GetBySubasta(int subastaId);

    /// <summary>Retorna la puja con el monto más alto de una subasta. Null si no hay pujas.</summary>
    Task<PujaDTO?> GetPujaMasAlta(int subastaId);

    /// <summary>
    /// Registra una puja con todas las validaciones de negocio.
    /// Retorna ok, mensaje y el DTO de la puja registrada si tuvo éxito.
    /// </summary>
    Task<(bool ok, string mensaje, PujaDTO? puja)> RegistrarPujaValidada(int subastaId, int usuarioId, decimal monto);
}
