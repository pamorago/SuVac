using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.Profiles
{
    public class AutorProfile : Profile
    {
        public AutorProfile()
        {
            CreateMap<Autor, AutorDTO>();
           /* CreateMap<Libro, LibroDTO>();
            CreateMap<Autor, AutorDTO>()
                .ForMember(d => d.Libros, opt => opt.MapFrom(s => s.Libro));*/
        }
    }
}
