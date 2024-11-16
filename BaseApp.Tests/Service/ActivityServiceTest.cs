using BaseApp.Constants;
using BaseApp.Data;
using BaseApp.DTO;
using BaseApp.Models;
using BaseApp.Repository;
using BaseApp.Repository.Base;
using BaseApp.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApp.Tests.Service
{
    public class ActivityServiceTest
    {

        private readonly IRepositoryManager _mockRepositoryManager;
        private readonly DbContextOptions<BaseAppDBContext> _mockDbContextOptions;
        private readonly BaseAppDBContext _dbContext;
        private Mock<ILocationService> _mockLocationService;
        private Mock<ICompanyInfoService> _mockCompanyInfoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ActivityService _activityService;

        public ActivityServiceTest() 
        {
            _mockDbContextOptions = new DbContextOptionsBuilder<BaseAppDBContext>()
                                        .UseInMemoryDatabase(databaseName: "ActivityTestBase")
                                        .EnableSensitiveDataLogging()
                                        .Options;
            _httpContextAccessor = new HttpContextAccessor();
            _dbContext = new BaseAppDBContext(_mockDbContextOptions, _httpContextAccessor);
            _mockRepositoryManager = new RepositoryManager(_dbContext);
            _mockLocationService = new Mock<ILocationService>();
            _mockCompanyInfoService = new Mock<ICompanyInfoService>();
            _activityService = new ActivityService(_mockRepositoryManager, _mockLocationService.Object, _mockCompanyInfoService.Object);
        }

        private async Task ResetDbContext()
        {
            _dbContext.EmployeeList.RemoveRange(_dbContext.EmployeeList);
            _dbContext.DeviceList.RemoveRange(_dbContext.DeviceList);
            _dbContext.LocationList.RemoveRange(_dbContext.LocationList);
            _dbContext.ActivityList.RemoveRange(_dbContext.ActivityList);
            await _dbContext.SaveChangesAsync();
        }

        private async Task PrepareData()
        {
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

            // set up an existed device in db
            DeviceModel device = new DeviceModel
            {
                Id = 1L,
                UUID = "1",
                Name = "Test",
                EmpId = 1,
            };
            _dbContext.DeviceList.Add(device);

            // set up an existed location in db
            LocationModel location = new LocationModel
            {
                Id = 1L,
                DeviceId = "1",
                Description = "In Office",
                Longtitude = 1.0,
                Latitude = 1.0,
                EmpId = 1L,
            };
            _dbContext.LocationList.Add(location);

            ActivityModel checkIn = new ActivityModel
            {
                Id = 1L,
                Type = Constants.EnumTypes.ActivityType.CHECKIN,
                ActivityTime = TimeOnly.FromDateTime(DateTime.Now),
                LocationId = 1L,
                ActiveFlag = true,
                DeleteFlag = false,
                CreatedDate = DateTime.Now,
                CreatedBy = "1",
                UpdatedDate = DateTime.Now,
                UpdatedBy = "1"
            };

            ActivityModel breakEnd = new ActivityModel
            {
                Id = 3L,
                Type = Constants.EnumTypes.ActivityType.BREAKEND,
                ActivityTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(4)),
                LocationId = 1L,
                ActiveFlag = true,
                DeleteFlag = false,
                CreatedDate = DateTime.Now.AddHours(4),
                CreatedBy = "1",
                UpdatedDate = DateTime.Now.AddHours(4),
                UpdatedBy = "1"
            };

            ActivityModel breakStart = new ActivityModel
            {
                Id = 2L,
                Type = Constants.EnumTypes.ActivityType.BREAKSTART,
                ActivityTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(2)),
                LocationId = 1L,
                ActiveFlag = true,
                DeleteFlag = false,
                CreatedDate = DateTime.Now.AddHours(2),
                CreatedBy = "1",
                UpdatedDate = DateTime.Now.AddHours(2),
                UpdatedBy = "1",
                ChildActivities = new List<ActivityModel> { breakEnd }
            };

            _dbContext.ActivityList.Add(checkIn);
            _dbContext.ActivityList.Add(breakEnd);
            _dbContext.ActivityList.Add(breakStart);
            await _dbContext.SaveChangesAsync();

            // Detach the entity to avoid tracking issues later
            _dbContext.Entry(checkIn).State = EntityState.Detached;

        }

        [Fact]
        public async Task GivenRightLocationId_ThenGetActivity_ShouldReturnNotNull()
        {

            // Arrange
            await PrepareData();
            long empId = 1L;
            double longtitude = 1.0, latitude = 1.0;
            
            // set up an existed location in db
            LocationModel existedLocation = new LocationModel
            {
                Id = 1L,
                DeviceId = "1",
                Description = "In Office",
                Longtitude = longtitude,
                Latitude = latitude,
                EmpId = empId,
            };

            _mockLocationService.Setup(service => service.GetAllByEmpIdAndLatAndLon(empId, latitude, longtitude))
                .ReturnsAsync(existedLocation);

            // Act
            ActivityModel result = await _activityService.GetEarliestCheckIn(empId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GivenWrongRequestActivityDTO_ThenCreateActivity_ShouldReturnFalse()
        {

            // Arrange
            await PrepareData();
            RequestActivityDTO requestActivityDTO = new RequestActivityDTO()
            {
                EmpId = 1,
                ActivityTime = "222",
                Longtitude = 2,
                Latitude = 2,
                Type = Constants.EnumTypes.ActivityType.CHECKIN
            };

            // Act
            bool createFunctionResult;
            try
            {
                createFunctionResult = await _activityService.CreateActivity(requestActivityDTO);
            }
            catch
            {
                createFunctionResult = false;
            }

            // Assert
            Assert.False(createFunctionResult);

            // validate that employee list in db mock contains the expected result
            ActivityModel createAcitvityinDb = _dbContext.ActivityList
                                                .FirstOrDefault(e =>
                                                e.Location.EmpId.Equals(requestActivityDTO.EmpId) &&
                                                e.ActivityTime.Equals(requestActivityDTO.ActivityTime) &&
                                                e.Location.Longtitude.Equals(requestActivityDTO.Longtitude) &&
                                                e.Location.Latitude.Equals(requestActivityDTO.Latitude));

            Assert.Null(createAcitvityinDb);

        }

        [Fact]
        public async Task GivenWrongEmpId_ThenSetNoteForActivity_ShouldThrowError()
        {
            // Arrange
            await PrepareData();

            // Act
            var setNoteFunctionResult = await Assert.ThrowsAsync<Exception>(() => _activityService.SetNoteForActivity(Constants.EnumTypes.ActivityType.BREAKSTART, "Xin nghỉ giữa giờ", 3));

            // Assert
            Assert.Equal(setNoteFunctionResult.Message, "Employee havent logged activity in day");
        }

        [Fact]
        public async Task GivenEmptyNote_ThenSetNoteForActivity_ShouldThrowError()
        {
            // Arrange
            await PrepareData();

            // Act
            var setNoteFunctionResult = await Assert.ThrowsAsync<Exception>(() => _activityService.SetNoteForActivity(Constants.EnumTypes.ActivityType.BREAKSTART, "", 1));

            // Assert
            Assert.Equal(setNoteFunctionResult.Message, "Note cannot be null");
        }


        [Fact]
        public async Task GivenCorrectInput_ThenSetNoteForAcitvity_ShouldSuccess()
        {
            // Arrange
            await PrepareData();

            // Act
            _activityService.SetNoteForActivity(Constants.EnumTypes.ActivityType.CHECKIN, "break start", 1);

            // Assert
            ActivityModel updatedActivityModel = await _mockRepositoryManager.activityRepostitory.FindByCondition(a => a.Type.Equals(EnumTypes.ActivityType.CHECKIN) && a.Location.EmpId == 1L).FirstOrDefaultAsync();

            Assert.NotNull(updatedActivityModel);

            Assert.Equal(updatedActivityModel.Note, "break start");
        }

    }
}
