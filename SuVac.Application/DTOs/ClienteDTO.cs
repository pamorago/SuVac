using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.DTOs
{
    public record ClienteDTO
    {
        public string IdCliente { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;

        public string Apellido1 { get; set; } = string.Empty;

        public string? Apellido2 { get; set; } = string.Empty;

        public string Sexo { get; set; } = string.Empty;

        public DateTime FechaNacimiento { get; set; }

        public List<OrdenDTO> Orden { get; set; } = new();
    }
}
