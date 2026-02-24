using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class Ganado
{
    public int GanadoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int TipoGanadoId { get; set; }

    public int RazaId { get; set; }

    public int SexoId { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public decimal PesoKg { get; set; }

    public string? CertificadoSalud { get; set; }

    public int EstadoGanadoId { get; set; }

    public DateTime FechaRegistro { get; set; }

    public int UsuarioVendedorId { get; set; }

    public virtual TipoGanado IdTipoGanadoNavigation { get; set; } = null!;

    public virtual Raza IdRazaNavigation { get; set; } = null!;

    public virtual Sexo IdSexoNavigation { get; set; } = null!;

    public virtual EstadoGanado IdEstadoGanadoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioVendedorNavigation { get; set; } = null!;

    public virtual ICollection<ImagenGanado> ImagenesGanado { get; set; } = new List<ImagenGanado>();

    public virtual ICollection<GanadoCategoria> GanadoCategorias { get; set; } = new List<GanadoCategoria>();

    public virtual ICollection<Subasta> Subastas { get; set; } = new List<Subasta>();
}
