using BaseApp.Constants;
using BaseApp.Models;
using BaseApp.Repository;
using BaseApp.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace BaseApp.Service
{

    public interface ILocationService
    {
        Task<LocationModel> GetLocationByLatAndLon(double lat, double lon);

        Task<List<LocationModel>> GetAllByEmpId(long empId);

        Task<LocationModel> GetAllByEmpIdAndLatAndLon(double empId, double lat, double lon);  

        Task<LocationModel> GetByEmpId(long empId);

        Task<string> CreateLocation(LocationModel location);    
    }

    public class LocationService : ILocationService
    {

        private readonly IRepositoryManager _repositoryManager;

        public LocationService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<LocationModel> GetLocationByLatAndLon(double lat, double lon) => await _repositoryManager.locationRepository.FindByCondition(location => 
                                                                                                                    location.Latitude == lat 
                                                                                                                    && location.Longtitude == lon)
                                                                                                                    .AsNoTracking().FirstOrDefaultAsync();

        public async Task<List<LocationModel>> GetAllByEmpId(long empId) => await _repositoryManager.locationRepository.FindByCondition(location => location.EmpId == empId)
                                                                                                                    .AsNoTracking().ToListAsync();

        public async Task<LocationModel> GetAllByEmpIdAndLatAndLon(double empId, double lat, double lon) => await _repositoryManager.locationRepository.FindByCondition(
                                                                                                                  location => location.EmpId == empId
                                                                                                                  && location.Longtitude == lon
                                                                                                                  && location.Latitude == lat).AsNoTracking().FirstOrDefaultAsync();

        public async Task<string> CreateLocation(LocationModel locationModel)
        {

            try
            {
                await _repositoryManager.locationRepository.Create(locationModel);
                await _repositoryManager.SaveAsync();
                return DevMessageConstants.ADD_OBJECT_SUCCESS;
            }
            catch (Exception ex)
            {

                return DevMessageConstants.ADD_OBJECT_FAILED + ". Ex: " + ex.Message;

            }
        }

        public async Task<LocationModel> GetByEmpId(long empId) => await _repositoryManager.locationRepository.FindByCondition(
                                                                                                                   location => location.EmpId == empId)
                                                                                                                   .AsNoTracking().FirstOrDefaultAsync();  

    }
}
