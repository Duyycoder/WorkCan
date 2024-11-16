using BaseApp.Data;
using BaseApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace BaseApp.Repository
{

    public interface IApplicationRepository : IRepositoryBase<ApplicationModel>
    {

    }

    public class ApplicationRepository : RepositoryBase<ApplicationModel>, IApplicationRepository
    {

        public ApplicationRepository(BaseAppDBContext dbContext) : base(dbContext) { }

    }
}
