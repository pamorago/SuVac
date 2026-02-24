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
    public class ClienteProfile: Profile
    {
        public ClienteProfile() {
            CreateMap<ClienteDTO, Cliente>().ReverseMap();
        }
    }
}
