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
            var @object = await _repository.GetById(id);
            var objectMapped = _mapper.Map<CategoriaDTO>(@object);
            return objectMapped;
        }

        public async Task<ICollection<CategoriaDTO>> ListAsync()
        {
            var list = await _repository.GetAll();
            return _mapper.Map<ICollection<CategoriaDTO>>(list);
        }

        public async Task<bool> Create(CategoriaDTO dto)
        {
            var categoria = _mapper.Map<Categoria>(dto);
            return await _repository.Create(categoria);
        }

        public async Task<bool> Update(CategoriaDTO dto)
        {
            var categoria = _mapper.Map<Categoria>(dto);
            return await _repository.Update(categoria);
        }

        public async Task<bool> Delete(int id)
        {
            return await _repository.Delete(id);
        }
    }
}
