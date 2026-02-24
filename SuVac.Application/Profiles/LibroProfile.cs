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
    public class LibroProfile: Profile
    {
        public LibroProfile() {
            //Entidad dto  (Listar detalles
            CreateMap<Libro, LibroDTO>().ReverseMap();


            // DTO - ENTIDAD (Crear u Actualizar 
            CreateMap<LibroDTO, Libro>().ReverseMap();
            
            CreateMap<LibroDTO, Libro>()
          
            .ForMember(dest => dest.IdAutorNavigation, orig => orig.Ignore())
            .ForMember(dest => dest.OrdenDetalle, orig => orig.MapFrom(o => o.OrdenDetalle))
            .ForMember(dest => dest.IdCategoria, orig => orig.MapFrom(o => o.IdCategoria));


        }
        
    }
}
