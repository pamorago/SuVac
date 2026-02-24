namespace SuVac.Application.DTOs;

public class ResultadoSubastaDTO
{
    public int ResultadoId { get; set; }
    public int SubastaId { get; set; }
    public int UsuarioGanadorId { get; set; }
    public decimal MontoFinal { get; set; }
    public DateTime FechaCierre { get; set; }
}
