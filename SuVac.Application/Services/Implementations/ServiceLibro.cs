using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;


namespace SuVac.Application.Services.Implementations
{
    public class ServiceLibro : IServiceLibro
    {
        private readonly IRepositoryLibro _repository;
        private readonly IMapper _mapper;

        public ServiceLibro(IRepositoryLibro repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> AddAsync(LibroDTO dto, string[] selectedCategorias)
        {
            try
            {
                var entity = _mapper.Map<Libro>(dto);
                return await _repository.AddAsync(entity, selectedCategorias);
            }
            catch (AutoMapperMappingException ex)
            {
                var msg = ex.ToString(); // incluye tipos origen/destino y qué miembro falló 
                throw;
            }

        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<LibroDTO> FindByIdAsync(int id)
        {
            var @object= await _repository.FindByIdAsync(id);
            var objectMapped = _mapper.Map<LibroDTO>(@object);
            return objectMapped;
        }

        public Task<ICollection<LibroDTO>> FindByNameAsync(string nombre)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<LibroDTO>> GetLibroByCategoria(int IdCategoria)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<LibroDTO>> ListAsync()
        {
            var list= await _repository.ListAsync();
            var collection=_mapper.Map< ICollection<LibroDTO>>(list);
            return collection;
        }

      
            public async Task UpdateAsync(int id, LibroDTO dto, string[] selectedCategorias)
        {
            // Traer entity (idealmente trackeado) antes de mapear encima 
            var entity = await _repository.FindByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"No existe el libro con id= {id} ");

            // Map "sobre" el entity existente (mantiene tracking) 
            _mapper.Map(dto, entity);

            await _repository.UpdateAsync(entity, selectedCategorias);
        }

    }
  }
   


