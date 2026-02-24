using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations
{
    public  class ServiceOrden: IServiceOrden
    {
        private readonly IRepositoryOrden _repositoryOrden;
        private readonly IMapper _mapper;
        public ServiceOrden(IRepositoryOrden repositoryOrden, IMapper mapper) { 
            _repositoryOrden= repositoryOrden;
            _mapper = mapper;
        }

        public Task<int> AddAsync(OrdenDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<OrdenDTO> FindByIdAsync(int id)
        {
            var @object = await _repositoryOrden.FindByIdAsync(id);
            var objectMapped = _mapper.Map<OrdenDTO>(@object);
            return objectMapped;
        }

        public async Task<OrdenDTO> FindByIdChangeAsync(int id)
        {
            var @object = await _repositoryOrden.FindByIdChangeAsync(id);
            var objectMapped = _mapper.Map<OrdenDTO>(@object);
            return objectMapped;
        }

        public async Task<int> GetNextNumberOrden()
        {
            int nextReceipt = await _repositoryOrden.GetNextNumberOrden();
            return nextReceipt + 1;
        }

        public async Task<ICollection<OrdenDTO>> ListAsync()
        {
            var list = await _repositoryOrden.ListAsync();
            var collection = _mapper.Map<ICollection<OrdenDTO>>(list);
            return collection;
        }

    }
}
