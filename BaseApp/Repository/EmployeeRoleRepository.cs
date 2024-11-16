using BaseApp.Data;
using BaseApp.Models;
using BaseApp.Repository.Base;

namespace BaseApp.Repository
{
    public interface IEmployeeRoleRepository : IRepositoryBase<EmpRoleModel>
    {

    }

    public class EmployeeRoleRepository : RepositoryBase<EmpRoleModel>, IEmployeeRoleRepository
    {

        public EmployeeRoleRepository(BaseAppDBContext dbContext) : base(dbContext) { }

    }
}
