using BaseApp.DTO;
using BaseApp.Models;
using BaseApp.Repository;
using Microsoft.EntityFrameworkCore;
using Serilog;
using BaseApp.Constants;
using Azure.Core;
using BaseApp.Repository.Base;
using System.Text.RegularExpressions;

namespace BaseApp.Service
{
    public interface IEmployeeService
    {
        Task<List<EmployeeModel>> GetAll();

        Task<EmployeeModel> GetById(long empId);

        Task<ResponseEmpDetailDTO> GetEmpProfile(long empId);

        Task<EmployeeModel> GetEmployeeByRefreshToken(string refreshToken);

        Task<EmployeeModel> GetEmployeeLoggedIn(long empId, string password);

        Task<bool> Create(RequestEmployeeDTO employeeDTO);

        Task<bool> Update(long empId, RequestEmployeeDTO employeeDTO);

        Task<bool> Delete(long empId);

        Task<bool> UpdatePassword(long empId, string newPassword);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repositoryManager;

        private readonly ICloudinaryService _cloudinaryService;

        public EmployeeService(IRepositoryManager repositoryManager, ICloudinaryService cloudinaryService)
        {
            _repositoryManager = repositoryManager;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<List<EmployeeModel>> GetAll() => await _repositoryManager.employeeRepository.FindAll().ToListAsync();

        public async Task<EmployeeModel> GetById(long id) => await _repositoryManager.employeeRepository.FindByCondition(emp => emp.Id == id).AsNoTracking().FirstOrDefaultAsync();

        public async Task<EmployeeModel> GetByEmail(string email) => await _repositoryManager.employeeRepository.FindByCondition(emp => emp.Email.Equals(email)).AsNoTracking().FirstOrDefaultAsync();

        public async Task<EmployeeModel> GetEmployeeLoggedIn(long empId, string password)
        {
            // check user with the username first, then proceed to check its password
            EmployeeModel emp = await _repositoryManager.employeeRepository.FindByCondition(emp => emp.Id == empId).AsNoTracking().FirstOrDefaultAsync();

            // if user cant be found with username and bcrypt cant verify password
            // throw new exception
            if (emp == null || !BCrypt.Net.BCrypt.Verify(password, emp.Password))
                throw new Exception("Username or Password is Incorrect");

            List<EmpRoleModel> empRoleList = await _repositoryManager.employeeRoleRepository.FindByCondition(empRole => empRole.EmployeeId == empId).Include(emp => emp.Role).AsNoTracking().ToListAsync();

            emp.EmpRoleList = empRoleList;

            // if not, return to the logged in user
            return emp;
        }

        public async Task<EmployeeModel> GetEmployeeByRefreshToken(string refreshToken)
        {

            // check user with the username first, then proceed to check its password
            EmployeeModel emp = await _repositoryManager.employeeRepository.FindByCondition(emp => emp.RefreshToken == refreshToken).AsNoTracking().FirstOrDefaultAsync();

            List<EmpRoleModel> empRoleList = await _repositoryManager.employeeRoleRepository.FindByCondition(empRole => empRole.EmployeeId == emp.Id).Include(emp => emp.Role).AsNoTracking().ToListAsync();

            emp.EmpRoleList = empRoleList;

            // if not, return to the logged in user
            return emp;

        }

        public virtual async Task<bool> Create(RequestEmployeeDTO employeeDTO)
        {
            var image = await _cloudinaryService.UploadImageAsync(employeeDTO.Avatar);

            if (image.Error != null)
            {
                throw new Exception("Failed to upload image. Error: " + image.Error);
            }

            // (?=.*[a-z]): require at least 1 lower case letter
            // (?=.*\d): require at least 1 number
            string regexPattern = "^(?=.*[a-z])(?=.*\\d)[a-z\\d]{6,}$";

            if (String.IsNullOrEmpty(employeeDTO.Password) || Regex.IsMatch(employeeDTO.Password, regexPattern))
            {
                throw new Exception("Password must be at least 6 char, contains at least 1 lower case letter and 1 number");
            }

            EmployeeModel employee = new EmployeeModel
            {
                FullName = employeeDTO.FullName,
                Password = BCrypt.Net.BCrypt.HashPassword(employeeDTO.Password),
                Email = employeeDTO.Email,
                PhoneNum = employeeDTO.PhoneNumber,
                Avatar = image.SecureUrl.ToString(),
                Address = employeeDTO.Address,
                GithubName = employeeDTO.GithubName,
            };

            await _repositoryManager.employeeRepository.Create(employee);

            await _repositoryManager.SaveAsync();

            return true;
        }

        public async Task<bool> Update(long empId, RequestEmployeeDTO employeeDTO)
        {

            EmployeeModel employee = await GetById(empId);

            if (employee == null)
            {
                throw new Exception(DevMessageConstants.OBJECT_NOT_FOUND);
            }

            if(employeeDTO.Avatar != null)
            {
                await _cloudinaryService.DeleteImageBySecureUrlAsync(employee.Avatar);

                var image = await _cloudinaryService.UploadImageAsync(employeeDTO.Avatar);

                if (image.Error != null)
                {
                    throw new Exception(image.Error.Message);
                }
                employee.Avatar = image.SecureUrl.ToString();
            }

            employee.FullName = employeeDTO.FullName;
            employee.Email = employeeDTO.Email;
            employee.PhoneNum = employeeDTO.PhoneNumber;
            employee.Address = employeeDTO.Address;
            employee.GithubName = employeeDTO.GithubName;

            try
            {
                _repositoryManager.employeeRepository.Update(employee);

                await _repositoryManager.SaveAsync();
            }
            catch
            {
                throw new Exception(DevMessageConstants.NOTIFICATION_UPDATE_FAILED);
            }

            return true;
        }

        public async Task<bool> Delete(long empId)
        {
            EmployeeModel employee = await GetById(empId);

            if (employee == null)
            {
                throw new Exception(DevMessageConstants.OBJECT_NOT_FOUND);
            }

            _repositoryManager.employeeRepository.Delete(employee);

            await _repositoryManager.SaveAsync();

            return true;
        }

        public async Task<bool> UpdatePassword(long empId, string newPassword)
        {
            EmployeeModel employeeModel = await GetById(empId);

            if(employeeModel == null)
            {
                throw new Exception(DevMessageConstants.OBJECT_NOT_FOUND);
            }

            employeeModel.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _repositoryManager.employeeRepository.Update(employeeModel);

            await _repositoryManager.SaveAsync();

            return true;
        }

        public async Task<ResponseEmpDetailDTO> GetEmpProfile(long empId)
        {
            EmployeeModel employee = await GetById(empId);
            return new ResponseEmpDetailDTO
            {
                EmpId = empId,
                Email = employee.Email,
                FullName = employee.FullName,
                GitHubName = employee.GithubName == null ? "" : employee.GithubName,
                Address = employee.Address,
                PhoneNumber = employee.PhoneNum,
                Avatar = employee.Avatar
            };
        }
    }
}
