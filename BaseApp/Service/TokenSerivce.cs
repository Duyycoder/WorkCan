using BaseApp.Configs;
using BaseApp.Constants;
using BaseApp.Data;
using BaseApp.DTO;
using BaseApp.Helpers;
using BaseApp.Models;
using BaseApp.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BaseApp.Service
{

    public interface ITokenService
    {
        Task<TokenResponse> Login(long empId, string password, bool isRemembermeActive, double longtitude, double latitude, string deviceId);

        Task<TokenResponse> RefreshToken(string refreshToken, bool isRemembermeActive);
    }

    public class TokenSerivce : ITokenService
    {
        private readonly string _symetricKey;
        private readonly string url = "https://localhost:7173";
        private readonly IEmployeeService _employeeService;
        private readonly ICompanyInfoService _companyInfoService;
        private readonly ILocationService _locationService;
        private readonly IRepositoryManager _repositoryManager;

        public TokenSerivce(IConfiguration configuration, IEmployeeService employeeService, ICompanyInfoService companyInfoService, ILocationService locationService, IRepositoryManager repositoryManager)
        {
            _employeeService = employeeService;
            _companyInfoService = companyInfoService;
            _locationService = locationService;
            _symetricKey = configuration["SymmetricKeys:MySymmetricKey"];
            _repositoryManager = repositoryManager;
        }

        private List<Claim> GenerateEmployeeClaims(EmployeeModel employee)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", employee.Id.ToString()),
                new Claim("Name", employee.FullName),
                new Claim("Email", employee.Email)
            };

            // add claim for each role of employee
            foreach (var empRole in employee.EmpRoleList)
            {
                claims.Add(new Claim("Roles", empRole.Role.Name.ToString()));
            }

            return claims;
        }

        private async Task UpdateRefreshToken(EmployeeModel employee, bool isRemembermeActive)
        {

            // if employee dont have refresh token or their token is expired
            // generate a new one
            string refreshToken = GenerateRefreshToken();
            employee.RefreshToken = refreshToken;
            employee.RefreshTokenExpiryTime = isRemembermeActive ? DateTime.Now.AddDays(7) : DateTime.Now.AddSeconds(20);
            _repositoryManager.employeeRepository.Update(employee);
            await _repositoryManager.SaveAsync();

        }

        public async Task<TokenResponse> Login(long empId, string password, bool isRemembermeActive, double longtitude, double latitude, string deviceId)
        {
            CalculateDistanceHelper calculateDistanceHelper = new CalculateDistanceHelper();

            DeviceModel existedDevice = await _repositoryManager.deviceRepository.FindByCondition(e => e.UUID.Equals(deviceId)).FirstOrDefaultAsync();
            
            // if device exists but dont have same empId, throw exception
            if (existedDevice != null)
            {
                if(existedDevice.EmpId != empId)
                {
                    throw new Exception("Employee's device is incorrect");
                }
            }

            // if device is not exists in db, create a new one
            else
            {
                DeviceModel newDevice = new DeviceModel 
                {
                    UUID = deviceId,
                    Name ="Auto Generated Device",
                    EmpId = empId,
                };
                await _repositoryManager.deviceRepository.Create(newDevice);
                await _repositoryManager.SaveAsync();
            }

            var emp = await _employeeService.GetEmployeeLoggedIn(empId, password);

            if (emp == null)
                throw new Exception("Username or password is not correct");

            if (!emp.ActiveFlag)
                throw new Exception($"Account {emp.Id} has been deactivated");

            var claims = GenerateEmployeeClaims(emp);

            var tokenResponse = GenerateAccessToken(claims, isRemembermeActive);

            // check if ip address matched with emp id exists in db first
            LocationModel existedLocationByEmpId = await _locationService.GetByEmpId(empId);
            if(existedLocationByEmpId == null)
            {
                LocationModel locationModel = new LocationModel
                {
                    DeviceId = deviceId,
                    Description = "In Office",
                    Longtitude = longtitude,
                    Latitude = latitude,
                    EmpId = empId,
                };

                await _locationService.CreateLocation(locationModel);
            }

            // check if location existed second
            LocationModel existedLocation = await _locationService.GetAllByEmpIdAndLatAndLon(empId, latitude, longtitude);

            string result = "";

            // create a new location when user log in if location is not existed
            if(existedLocation == null)
            {

                LocationModel newLocation = new LocationModel
                {
                    EmpId = empId,
                    DeviceId = deviceId,
                    Longtitude = longtitude,
                    Latitude = latitude,
                    Description = "In Office",
                };

                result = await _locationService.CreateLocation(newLocation);
            }

            // if refresh token is null  or expire, generate a new one
            if (emp.RefreshTokenExpiryTime == null || emp.RefreshTokenExpiryTime <= DateTime.Now || emp.RefreshToken == null)
            {
                await UpdateRefreshToken(emp, isRemembermeActive);
            }

            tokenResponse.RefreshToken = emp.RefreshToken;

            tokenResponse.RefreshTokenExpiryDate = emp.RefreshTokenExpiryTime;

            tokenResponse.EmpId = empId;

            tokenResponse.FullName = emp.FullName;

            return tokenResponse;
        }

        public async Task<TokenResponse> RefreshToken(string refreshToken, bool isRemembermeActive)
        {
            EmployeeModel employee = await _employeeService.GetEmployeeByRefreshToken(refreshToken);

            if (employee == null || employee.RefreshTokenExpiryTime <= DateTime.Now)
                throw new Exception(DevMessageConstants.TOKEN_EXPIRED);

            if (!employee.ActiveFlag)
                throw new Exception(String.Format(DevMessageConstants.DATA_WAS_DELETED, employee.Id.ToString()));

            var claims = GenerateEmployeeClaims(employee);

            var newAccessToken = GenerateAccessToken(claims, isRemembermeActive);

            newAccessToken.RefreshToken = employee.RefreshToken;

            newAccessToken.RefreshTokenExpiryDate = employee.RefreshTokenExpiryTime;

            return newAccessToken;
        }


        private TokenResponse GenerateAccessToken(IEnumerable<Claim> claims, bool isRememberMe)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_symetricKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokeOptions = new JwtSecurityToken(
                issuer: url,
                audience: url,
                claims: claims,
                expires: DateTime.Now.AddSeconds(10),
                signingCredentials: signinCredentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            TokenResponse result = new TokenResponse
            {
                AccessToken = tokenString,
                AccessTokenExpiryDate = tokeOptions.ValidTo.ToLocalTime()
            };

            return result;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

    }
}
