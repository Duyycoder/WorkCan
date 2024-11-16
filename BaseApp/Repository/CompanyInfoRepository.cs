using BaseApp.Data;
using BaseApp.Models;
using BaseApp.Repository.Base;

namespace BaseApp.Repository
{
    public interface ICompanyInfoRepository : IRepositoryBase<CompanyInfoModel>
    {
        
    }

    public class CompanyInfoRepository : RepositoryBase<CompanyInfoModel>, ICompanyInfoRepository
    {

        public CompanyInfoRepository(BaseAppDBContext dbContext) : base(dbContext) { }

    }
}
