using BaseApp.Data;
using BaseApp.Models;
using BaseApp.Repository.Base;

namespace BaseApp.Repository
{

    public interface ILocationRepository : IRepositoryBase<LocationModel>
    {

    }

    public class LocationRepository : RepositoryBase<LocationModel>, ILocationRepository
    {

        public LocationRepository(BaseAppDBContext baseAppDBContext) : base(baseAppDBContext)  { }


    }
}
