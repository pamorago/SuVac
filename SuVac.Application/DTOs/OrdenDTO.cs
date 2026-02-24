using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.DTOs
{
    public record OrdenDTO
    {
        [Display(Name = "Orden #")]
        public int IdOrden { get; set; }

        [Display(Name = "Cliente")]
        public string IdCliente { get; set; } = string.Empty;

        [Display(Name = "Usuario")]
        public int IdUsuario { get; set; }

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime FechaOrden { get; set; }

        public decimal Total { get; set; }

        [Display(Name = "Cliente")]
        public ClienteDTO IdClienteNavigation { get; set; } = new();

        public UsuarioDTO IdUsuarioNavigation { get; set; } = new();

        public List<OrdenDetalleDTO> OrdenDetalle { get; set; } = new();
    }
}
