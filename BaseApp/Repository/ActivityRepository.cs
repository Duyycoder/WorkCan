using BaseApp.Data;
using BaseApp.Models;
using BaseApp.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace BaseApp.Repository
{

    public interface IActivityRepostitory : IRepositoryBase<ActivityModel>
    {

    }

    public class ActivityRepository : RepositoryBase<ActivityModel>, IActivityRepostitory
    {

        public ActivityRepository(BaseAppDBContext dBContext) : base(dBContext) { }

    }
}
