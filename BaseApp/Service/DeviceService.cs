using BaseApp.Models;
using BaseApp.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace BaseApp.Service
{

    public interface IDeviceService
    {


    }

    public class DeviceService : IDeviceService
    {

        private readonly IRepositoryManager _repositoryManager;

        public DeviceService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

    }
}
