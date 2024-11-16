using BaseApp.Data;
using BaseApp.Models;
using BaseApp.Repository.Base;

namespace BaseApp.Repository
{
    public interface IRoleRepository : IRepositoryBase<RoleModel>
    {

    }

    public class RoleRepository : RepositoryBase<RoleModel>, IRoleRepository
    {

        public RoleRepository(BaseAppDBContext baseAppDBContext) : base(baseAppDBContext) { }

    }
}
