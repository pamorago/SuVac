using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.Services.Implementations
{
    public class ServiceAutor : IServiceAutor
    {
        private readonly IRepositoryAutor _repository;
        private readonly IMapper _mapper;

        public ServiceAutor(IRepositoryAutor repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AutorDTO?> FindByIdAsync(int id)
        {
            var @object= await _repository.FindByIdAsync(id);
            var objectMapped=_mapper.Map<AutorDTO>(@object);
            return objectMapped;
        }

        public async Task<ICollection<AutorDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<AutorDTO>>(list);
        }
    }
}
