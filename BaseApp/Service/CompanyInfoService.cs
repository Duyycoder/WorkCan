using BaseApp.Models;
using BaseApp.Repository;
using BaseApp.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace BaseApp.Service
{

    public interface ICompanyInfoService 
    {
        Task<List<CompanyInfoModel>> GetAll();    

        Task<CompanyInfoModel> GetById(int id);
    }


    public class CompanyInfoService : ICompanyInfoService
    {

        private readonly IRepositoryManager _repositoryManager;

        public CompanyInfoService(IRepositoryManager repositoryManager) 
        {
            this._repositoryManager = repositoryManager;
        }

        public async Task<List<CompanyInfoModel>> GetAll() => await _repositoryManager.companyInfoRepository.FindAll().AsNoTracking().ToListAsync();

        public async Task<CompanyInfoModel> GetById(int id) => await _repositoryManager.companyInfoRepository.FindByCondition(c => c.Id == id).FirstOrDefaultAsync();

    }
}
