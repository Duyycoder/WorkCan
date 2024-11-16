using BaseApp.Data;
using BaseApp.Models;
using BaseApp.Repository.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaseApp.Repository
{
    public interface IEmployeeRepository : IRepositoryBase<EmployeeModel>
    {

    }

    public class EmployeeRepository : RepositoryBase<EmployeeModel>, IEmployeeRepository
    {

        public EmployeeRepository(BaseAppDBContext dbContext) : base(dbContext) { }

    }
}
