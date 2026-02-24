using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.DTOs
{
    public record AutorDTO
    {
        [DisplayName("Identificador Autor")]
        public int IdAutor { get; set; }

        [DisplayName("Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Libros")]
        public List<LibroDTO> Libros { get; set; } = new();
    }
}
