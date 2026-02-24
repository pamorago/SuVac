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
    public class OrdenProfile: Profile
    {
        public OrdenProfile() {
            CreateMap<OrdenDTO, Orden>().ReverseMap();
        }
    }
}
