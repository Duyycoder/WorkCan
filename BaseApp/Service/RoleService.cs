using BaseApp.Constants;
using BaseApp.Models;
using BaseApp.Repository;
using BaseApp.Repository.Base;

namespace BaseApp.Service
{
    public interface IRoleService
    {
        List<RoleModel> GetAll();

        Task<bool> AddRoleAsync(string roleName);
    }

    public class RoleService : IRoleService
    {

        private readonly IRepositoryManager _repositoryManager;

        public RoleService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

        public List<RoleModel> GetAll() => _repositoryManager.roleRepository.FindAll().ToList();

        public async Task<bool> AddRoleAsync(string name)
        {

            // check if input string matches one of the role names
            if(Enum.TryParse(name, true, out EnumTypes.RoleType roleName))
            {
                RoleModel newRole = new RoleModel
                {
                    Name = roleName
                };

                await _repositoryManager.roleRepository.Create(newRole);

                await _repositoryManager.SaveAsync();

                return true;
            }

            else
            {
                throw new ArgumentException("Invalid role name");
            }

        }

    }
}
