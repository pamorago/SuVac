using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.DTOs
{
    public record LibroDTO
    {
        [DisplayName("Identificador Libro")]
        public int IdLibro { get; set; }

        [DisplayName("ISBN")]
        [Required(ErrorMessage ="{0} es un dato requerido")]
        [StringLength(20, MinimumLength =4, ErrorMessage ="{0} debe tener entre {2} y {1} caracteres")]
        public string Isbn { get; set; } = string.Empty;

        [DisplayName("Autor")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range (1,int.MaxValue, ErrorMessage ="Debe seleccionar un {0}")]
        public int IdAutor { get; set; }

        [Display(Name = "Nombre Libro")]
        //reglas de validacion 
        [Required(ErrorMessage = "{0} es un dato requerido")]

        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        
        public decimal Precio { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        [Range(0, 99999, ErrorMessage = "El {0} debe esta entre {1} y {2}")]
        public int Cantidad { get; set; }

        [Display(Name = "Imagen Libro")]
        public byte[] Imagen { get; set; } = Array.Empty<byte>();

        [DisplayName("Autor")]
        public AutorDTO IdAutorNavigation { get; set; } = new();

        public List<OrdenDetalleDTO> OrdenDetalle { get; set; } = new();

        [Display(Name = "Categoría")]
        public List<CategoriaDTO> IdCategoria { get; set; } = new();
    }
}
