using BaseApp.Data;

namespace BaseApp.Repository.Base
{

    public interface IRepositoryManager
    {

        IActivityRepostitory activityRepostitory { get; }

        ICompanyInfoRepository companyInfoRepository { get; }

        IEmployeeRepository employeeRepository { get; } 

        IEmployeeRoleRepository employeeRoleRepository { get; }

        ILocationRepository locationRepository { get; }

        IRoleRepository roleRepository { get; }

        IDeviceRepository deviceRepository { get; }

        IApplicationRepository applicationRepository { get; }

        Task SaveAsync();

        void Dispose();
        
    }

    public class RepositoryManager : IRepositoryManager
    {

        private readonly BaseAppDBContext _dbContext;

        private readonly IActivityRepostitory _activityRepostitory;

        private readonly ICompanyInfoRepository _companyInfoRepository;

        private readonly IEmployeeRepository _employeeRepository;

        private readonly IEmployeeRoleRepository _employeeRoleRepository;

        private readonly ILocationRepository _locationRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IDeviceRepository _deviceRepository;

        private readonly IApplicationRepository _applicationRepository;

        public RepositoryManager(BaseAppDBContext context)
        {
            _dbContext = context;
        }

        public IActivityRepostitory activityRepostitory => _activityRepostitory ?? new ActivityRepository(_dbContext);

        public ICompanyInfoRepository companyInfoRepository => _companyInfoRepository ?? new CompanyInfoRepository(_dbContext);

        public IEmployeeRepository employeeRepository => _employeeRepository ?? new EmployeeRepository(_dbContext); 

        public IEmployeeRoleRepository employeeRoleRepository => _employeeRoleRepository ?? new EmployeeRoleRepository(_dbContext);  

        public ILocationRepository locationRepository => _locationRepository ?? new LocationRepository(_dbContext);

        public IRoleRepository roleRepository => _roleRepository ?? new RoleRepository(_dbContext);

        public IDeviceRepository deviceRepository => _deviceRepository ?? new DeviceRepository(_dbContext);

        public IApplicationRepository applicationRepository => _applicationRepository ?? new ApplicationRepository(_dbContext);

        public virtual async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose() => _dbContext.Dispose();
    }
}
