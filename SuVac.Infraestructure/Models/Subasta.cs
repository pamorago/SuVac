using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class Subasta
{
    public int SubastaId { get; set; }

    public int GanadoId { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public decimal PrecioBase { get; set; }

    public decimal IncrementoMinimo { get; set; }

    public int EstadoSubastaId { get; set; }

    public int UsuarioCreadorId { get; set; }

    public virtual Ganado IdGanadoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioCreadorNavigation { get; set; } = null!;

    public virtual EstadoSubasta IdEstadoSubastaNavigation { get; set; } = null!;

    public virtual ICollection<Puja> Pujas { get; set; } = new List<Puja>();

    public virtual ICollection<ResultadoSubasta> ResultadoSubasta { get; set; } = new List<ResultadoSubasta>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
