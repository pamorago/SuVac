using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.Services.Implementations
{
    public class ServiceCategoria : IServiceCategoria
    {
        private readonly IRepositoryCategoria _repository;
        private readonly IMapper _mapper;

        public ServiceCategoria(IRepositoryCategoria repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CategoriaDTO?> FindByIdAsync(int id)
        {
            var @object= await _repository.FindByIdAsync(id);
            var objectMapped=_mapper.Map<CategoriaDTO>(@object);
            return objectMapped;
        }

        public async Task<ICollection<CategoriaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<CategoriaDTO>>(list);
        }

        
    }
}
