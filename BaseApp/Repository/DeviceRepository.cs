using BaseApp.Data;
using BaseApp.Models;

namespace BaseApp.Repository
{

    public interface IDeviceRepository : IRepositoryBase<DeviceModel>
    {

    }

    public class DeviceRepository : RepositoryBase<DeviceModel>, IDeviceRepository
    {
        public DeviceRepository(BaseAppDBContext context) : base(context) { }
    }
}
