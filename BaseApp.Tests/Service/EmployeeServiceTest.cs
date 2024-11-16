using BaseApp.DTO;
using BaseApp.Models;
using BaseApp.Repository;
using BaseApp.Service;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using MockQueryable;
using static System.Net.Mime.MediaTypeNames;
using BaseApp.Repository.Base;
using BaseApp.Constants;
using System.Text;
using BaseApp.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serilog;

namespace BaseApp.Tests.Service
{
    public class EmployeeServiceTest
    {
        private readonly IRepositoryManager _mockRepositoryManager;
        private readonly Mock<ICloudinaryService> _mockCloudinaryService;
        private readonly DbContextOptions<BaseAppDBContext> _dbContextOptions;
        private readonly BaseAppDBContext _dbContext;
        private readonly EmployeeService _employeeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeServiceTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BaseAppDBContext>()
                                    .UseInMemoryDatabase(databaseName: "EmployeeTestBase")
                                    .EnableSensitiveDataLogging()
                                    .Options;
            _httpContextAccessor = new HttpContextAccessor();
            _dbContext = new BaseAppDBContext(_dbContextOptions, _httpContextAccessor);
            _mockRepositoryManager = new RepositoryManager(_dbContext);
            _mockCloudinaryService = new Mock<ICloudinaryService>();
            _employeeService = new EmployeeService(_mockRepositoryManager, _mockCloudinaryService.Object);
        }

        private async Task ResetDbContext()
        {
            _dbContext.EmployeeList.RemoveRange(_dbContext.EmployeeList);
            await _dbContext.SaveChangesAsync();
        }

        private async Task GenerateEmployeeInDb()
        {
            // reset db after each test
            await ResetDbContext();

            // set up an existed employee in db
            EmployeeModel employee = new EmployeeModel
            {
                Id = 1,
                Code = "Code1",
                FullName = "superadmin",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Email = "superadmin@gmail.com",
                PhoneNum = "+84388124368",
                Avatar = "https://res.cloudinary.com/duylinhmedia/image/upload/v1725848701/rje2a5ibauortais1xrl.jpg",
                Address = "Ha Noi",
                CreatedDate = DateTime.Now,
                CreatedBy = 1.ToString(),
                UpdatedDate = DateTime.Now,
                UpdatedBy = 1.ToString()
            };

            _dbContext.EmployeeList.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Detach the entity to avoid tracking issues later
            _dbContext.Entry(employee).State = EntityState.Detached;
        }


        [Fact]
        public async Task GivenCorrectEmployeeDTO_ThenCreateEmployee_ShouldReturnTrue()
        {

            // Arrange
            await GenerateEmployeeInDb();
            RequestEmployeeDTO employeeDTO = new RequestEmployeeDTO
            {
                FullName = "linh ND",
                GithubName = "linhnd",
                Password = "Password123",
                PhoneNumber = "+84388124368",
                Email = "linh@gmail.com",
                Avatar = new Mock<IFormFile>().Object,
                Address = "123 Main St",
            };

            var imageUploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri("https://example.com/image.jpg")
            };

            _mockCloudinaryService.Setup(service => service.UploadImageAsync(employeeDTO.Avatar))
                .ReturnsAsync(imageUploadResult);

            // Act
            bool createFunctionResult;
            try
            {
                createFunctionResult = await _employeeService.Create(employeeDTO);
            } 
            catch
            {
                createFunctionResult = false;
            }

            // Assert
            Assert.True(createFunctionResult);

            // validate that employee list in db mock contains the expected result
            EmployeeModel createdEmployeeInDb = _dbContext.EmployeeList
                                                .FirstOrDefault(e =>
                                                e.FullName.Equals(employeeDTO.FullName) &&
                                                e.PhoneNum.Equals(employeeDTO.PhoneNumber) &&
                                                e.Email.Equals(employeeDTO.Email) &&
                                                e.Address.Equals(employeeDTO.Address));

            Assert.NotNull(createdEmployeeInDb);

        }

        [Fact]
        public async Task GivenWrongEmployeeDTO_ThenCreateEmployee_ShouldReturnFalse()
        {

            // Arrange
            await GenerateEmployeeInDb();
            RequestEmployeeDTO employeeDTO = new RequestEmployeeDTO
            {
                Avatar = new Mock<IFormFile>().Object,
                Address = "123 Main St",
            };

            var imageUploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri("https://example.com/image.jpg")
            };

            _mockCloudinaryService.Setup(service => service.UploadImageAsync(employeeDTO.Avatar))
                .ReturnsAsync(imageUploadResult);

            // Act
            bool createFunctionResult;
            try
            {
                createFunctionResult = await _employeeService.Create(employeeDTO);
            }
            catch
            {
                createFunctionResult = false;
            }

            // Assert
            Assert.False(createFunctionResult);

            // validate that employee list in db mock contains the expected result
            EmployeeModel createdEmployeeInDb = _dbContext.EmployeeList
                                                .FirstOrDefault(e =>
                                                e.FullName.Equals(employeeDTO.FullName) &&
                                                e.PhoneNum.Equals(employeeDTO.PhoneNumber) &&
                                                e.Email.Equals(employeeDTO.Email) &&
                                                e.Address.Equals(employeeDTO.Address));

            Assert.Null(createdEmployeeInDb);

        }

        [Fact]
        public async Task GivenCorrectEmpIdAndEmployeeDTO_ThenUpdateEmployee_ShouldReturnTrue()
        {

            // Arrange
            await GenerateEmployeeInDb();
            RequestEmployeeDTO employeeDTO = new RequestEmployeeDTO
            {
                FullName = "linh ND",
                GithubName = "linh123",
                Password = "Password123",
                PhoneNumber = "+84388124368",
                Email = "admin@gmail.com", // update the email field
                Avatar = new Mock<IFormFile>().Object,
                Address = "123 Main St",
            };

            var imageUploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri("https://example.com/image.jpg")
            };

            _mockCloudinaryService.Setup(service => service.UploadImageAsync(employeeDTO.Avatar))
                .ReturnsAsync(imageUploadResult);

            // Act
            bool updateFunctionResult;
            try
            {
                updateFunctionResult = await _employeeService.Update(1L, employeeDTO);
            }
            catch
            {
                updateFunctionResult = false;
            }

            // Assert
            Assert.True(updateFunctionResult);

            // validate that employee list in db mock contains the expected result
            EmployeeModel updatedEmployeeInDb = _dbContext.EmployeeList
                                                .FirstOrDefault(e =>
                                                e.Id == 1);

            Assert.Equal(updatedEmployeeInDb.Email, employeeDTO.Email);
        }

        [Fact]
        public async Task GivenWrongEmpIdAndCorrectEmployeeDTO_ThenUpdateEmployee_ShouldReturnFalse()
        {

            // Arrange
            await GenerateEmployeeInDb();
            long employeeId = 4444L;
            RequestEmployeeDTO employeeDTO = new RequestEmployeeDTO
            {
                FullName = "linh ND",
                Password = "password123",
                Email = "superadmin@gmail.com",
                PhoneNumber = "1111",
                Avatar = new Mock<IFormFile>().Object,
                Address = "123 Main St",
            };

            var imageUploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri("https://example.com/image.jpg")
            };

            _mockCloudinaryService.Setup(service => service.UploadImageAsync(employeeDTO.Avatar))
                .ReturnsAsync(imageUploadResult);

            var expectedResult = new Exception(DevMessageConstants.OBJECT_NOT_FOUND);

            List<EmployeeModel> existedEmployee = new List<EmployeeModel>();

            // Act
            var updateFunctionResult = await Assert.ThrowsAsync<Exception>(() => _employeeService.Update(employeeId, employeeDTO));

            // Assert
            Assert.Equal(expectedResult.Message, updateFunctionResult.Message);
        }

        [Fact]
        public async Task GivenCorrectEmpId_ThenDeleteEmployee_ShouldReturnTrue()
        {

            // Arrange
            await GenerateEmployeeInDb();

            List<EmployeeModel> existedEmployee = new List<EmployeeModel>();

            // Act
            bool deleteFunctionResult =  await _employeeService.Delete(1L);

            // Assert
            Assert.True(deleteFunctionResult);


        }

        [Fact]
        public async Task GivenWrongEmpId_ThenDeleteEmployee_ShouldReturnFalse()
        {

            // Arrange
            await GenerateEmployeeInDb();

            var expectedResult = new Exception(DevMessageConstants.OBJECT_NOT_FOUND);

            List<EmployeeModel> existedEmployee = new List<EmployeeModel>();

            // Act
            var deleteFunctionResult = await Assert.ThrowsAsync<Exception>(() => _employeeService.Delete(4444L));

            // Assert
            Assert.Equal(expectedResult.Message, deleteFunctionResult.Message);
        }

        [Fact]
        public async Task GivenFileTooSmall_ThenUploadFile_ShouldThrowError()
        {
            // Arange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1 * 512);
            mockFile.Setup(f => f.ContentType).Returns("image/jpg");

            var expectedResult = new ImageUploadResult
            {
                Error = new Error { Message = "File's size is at least 1kb." }
            };

            _mockCloudinaryService
                .Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _mockCloudinaryService.Object.UploadImageAsync(mockFile.Object);

            // Assert
            Assert.NotNull(result.Error);
            Assert.Equal(expectedResult.Error.Message, result.Error.Message);
        }

        [Fact]
        public async Task GivenFileTooLarge_ThenUploadFile_ShouldThrowError()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(20 * 1024 * 1024);
            mockFile.Setup(f => f.ContentType).Returns("image/jpg");

            var expectedResult = new ImageUploadResult
            {
                Error = new Error { Message = "File's size cannot be more than 10mb." }
            };

            _mockCloudinaryService
                .Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _mockCloudinaryService.Object.UploadImageAsync(mockFile.Object);

            // Assert
            Assert.NotNull(result.Error);
            Assert.Equal(expectedResult.Error.Message, result.Error.Message);
        }

        [Fact]
        public async Task GivenInvalidFileType_ThenUploadFile_ShouldThrowError()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(5 * 1024 * 1024);
            mockFile.Setup(f => f.ContentType).Returns("application/pdf");

            var expectedResult = new ImageUploadResult
            {
                Error = new Error { Message = "File's type should only be image/jpeg, image/png, image/jpg, image/gif." }
            };

            _mockCloudinaryService
                .Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _mockCloudinaryService.Object.UploadImageAsync(mockFile.Object);

            // Assert
            Assert.NotNull(result.Error);
            Assert.Equal(expectedResult.Error.Message, result.Error.Message);
        }

    }
}
