using BaseApp.Constants;
using BaseApp.Data;
using BaseApp.DTO;
using BaseApp.Helpers;
using BaseApp.Models;
using BaseApp.Repository.Base;
using BaseApp.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BaseApp.Tests.Service
{
    public class ApplicationServiceTest
    {
        private readonly BaseAppDBContext _context;
        private readonly ApplicationService _applicationService;
        private readonly IRepositoryManager _repositoryManager;
        private readonly DbContextOptions<BaseAppDBContext> _dbContextOptions;

        public ApplicationServiceTest()
        {
            // Thiết lập In-Memory Database cho DbContext
            _dbContextOptions = new DbContextOptionsBuilder<BaseAppDBContext>()
                .UseInMemoryDatabase(databaseName: "ApplicationTestBase")
                .EnableSensitiveDataLogging()
                .Options;

            var httpContextAccessor = new HttpContextAccessor();
            _context = new BaseAppDBContext(_dbContextOptions, httpContextAccessor);

            if (_context == null)
            {
                throw new ArgumentNullException(nameof(_context));
            }

            _repositoryManager = new RepositoryManager(_context);
            _applicationService = new ApplicationService(_repositoryManager);
        }

        private async Task ResetDbContext()
        {
            _context.ApplicationList.RemoveRange(_context.ApplicationList);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateApplication_ValidInput_ShouldReturnTrue()
        {
            // Reset DbContext để đảm bảo không có dữ liệu trước đó
            await ResetDbContext();

            // Arrange
            var requestApplicationDTO = new RequestApplicationDTO
            {
                EmpId = 1L,
                Title = "Test Application",
                ApplicationType = EnumTypes.ApplicationType.LEAVE,
                StartedDate = DateTime.Now.Date,
                EndedDate = DateTime.Now.Date.AddDays(2),
                Note = "This is a test application"
            };

            // Act
            var result = await _applicationService.CreateApplication(requestApplicationDTO);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateApplication_InvalidInput_ShouldReturnFalse()
        {
            // Reset DbContext để đảm bảo không có dữ liệu trước đó
            await ResetDbContext();

            // Arrange
            var requestApplicationDTO = new RequestApplicationDTO
            {
                EmpId = 1L,
                Title = "Test Application",
                ApplicationType = EnumTypes.ApplicationType.LEAVE,
                StartedDate = DateTime.Now.Date,
                EndedDate = DateTime.Now.Date.AddDays(2),
                Note = "This is a test application"
            };

            // Giả lập ngoại lệ
            var mockRepositoryManager = new Mock<IRepositoryManager>();
            mockRepositoryManager.Setup(m => m.applicationRepository.Create(It.IsAny<ApplicationModel>()))
                .Throws(new Exception());
            mockRepositoryManager.Setup(m => m.SaveAsync())
                .Throws(new Exception());

            var applicationService = new ApplicationService(mockRepositoryManager.Object);

            // Act
            var result = await applicationService.CreateApplication(requestApplicationDTO);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetApplicationList_FiltersApplied_ReturnsFilteredList()
        {
            // Reset DbContext để đảm bảo không có dữ liệu trước đó
            await ResetDbContext();

            // Arrange
            var empId = 1L;
            var type = EnumTypes.ApplicationType.LEAVE;
            var page = 1;
            var size = 10;
            var from = DateTime.Now.Date.AddDays(-1);
            var to = DateTime.Now.Date.AddDays(1);

            var applications = new List<ApplicationModel>
            {
                new ApplicationModel
                {
                    // Để Entity Framework tự động gán ID
                    EmployeeId = empId,
                    Type = type,
                    Name = "Application Name",
                    Note = "Application Note",
                    StartDate = DateTime.Now.Date,
                    EndDate = DateTime.Now.Date.AddDays(1),
                    CreatedDate = DateTime.Now
                }
            };

            await _context.ApplicationList.AddRangeAsync(applications);
            await _context.SaveChangesAsync();

            // Act
            var result = await _applicationService.GetApplicationList(empId, type, page, size, from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(page, result.PageNumber);
            Assert.Equal(size, result.PageSize);
            Assert.Equal(applications.Count, result.TotalRecords);
            Assert.Equal(applications.Count, result.ApplicationList.Count);
        }

        [Fact]
        public async Task UpdateApplicationStatus_ValidStatusChange_ShouldUpdateSuccessfully()
        {
            await ResetDbContext();

            // Arrange
            var applicationModel = new ApplicationModel
            {
                Name = "Test Application",
                Note = "Application Note",
                Status = EnumTypes.ApplicationStatus.REQUESTED
            };

            await _context.ApplicationList.AddAsync(applicationModel);
            await _context.SaveChangesAsync();

            // Lấy ID mới gán tự động
            var assignedId = applicationModel.Id;
            var requestUpdateApplicationStatusDTO = new RequestUpdateApplicationStatusDTO
            {
                ApplicationId = assignedId,
                ApplicationStatus = EnumTypes.ApplicationStatus.APPROVED
            };

            // Tách thực thể ra khỏi `DbContext` để đảm bảo không có thực thể trùng lặp được theo dõi
            _context.Entry(applicationModel).State = EntityState.Detached;

            // Act
            await _applicationService.UpdateApplicationStatus(requestUpdateApplicationStatusDTO);

            // Assert
            var updatedApplication = await _context.ApplicationList.FindAsync(assignedId);
            Assert.Equal(EnumTypes.ApplicationStatus.APPROVED, updatedApplication.Status);
        }



        [Fact]
        public async Task UpdateApplicationStatus_InvalidStatusChange_ShouldThrowException()
        {
            await ResetDbContext();

            // Arrange
            var applicationId = 1L;
            var newStatus = EnumTypes.ApplicationStatus.REQUESTED;
            var requestUpdateApplicationStatusDTO = new RequestUpdateApplicationStatusDTO
            {
                ApplicationId = applicationId,
                ApplicationStatus = newStatus
            };

            var applicationModel = new ApplicationModel
            {
                // Để Entity Framework tự động gán ID
                Name = "Test Application",
                Note = "Application Note",
                Status = EnumTypes.ApplicationStatus.APPROVED
            };

            await _context.ApplicationList.AddAsync(applicationModel);
            await _context.SaveChangesAsync();

            // Lấy ID mới gán tự động
            var assignedId = applicationModel.Id;
            requestUpdateApplicationStatusDTO.ApplicationId = assignedId;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await _applicationService.UpdateApplicationStatus(requestUpdateApplicationStatusDTO));
        }

        [Fact]
        public async Task GetYearlyStatistics_ValidData_ShouldReturnStatistics()
        {
            await ResetDbContext();

            // Arrange
            var empId = 1L;
            var year = 2024;

            var approvedLeaveApplications = new List<ApplicationModel>
    {
        new ApplicationModel
        {
            EmployeeId = empId,
            Type = EnumTypes.ApplicationType.LEAVE,
            Status = EnumTypes.ApplicationStatus.APPROVED,
            Name = "Leave Application",
            Note = "Approved Leave",
            StartDate = new DateTime(year, 1, 1),
            EndDate = new DateTime(year, 1, 2),
            CreatedDate = new DateTime(year, 1, 1)
        }
    };

            var requestedOvertimeListInMonth = new List<ApplicationModel>
    {
        new ApplicationModel
        {
            EmployeeId = empId,
            Type = EnumTypes.ApplicationType.OVERTIME,
            Status = EnumTypes.ApplicationStatus.REQUESTED,
            Name = "Overtime Request",
            Note = "Requested Overtime",
            StartDate = new DateTime(year, 1, 1, 9, 0, 0),
            EndDate = new DateTime(year, 1, 1, 18, 0, 0),
            CreatedDate = new DateTime(year, 1, 1)
        }
    };

            var approvedOvertimeListInMonth = new List<ApplicationModel>
    {
        new ApplicationModel
        {
            EmployeeId = empId,
            Type = EnumTypes.ApplicationType.OVERTIME,
            Status = EnumTypes.ApplicationStatus.APPROVED,
            Name = "Overtime Approval",
            Note = "Approved Overtime",
            StartDate = new DateTime(year, 1, 1, 9, 0, 0),
            EndDate = new DateTime(year, 1, 1, 18, 0, 0),
            CreatedDate = new DateTime(year, 1, 1)
        }
    };

            var remoteApplicationList = new List<ApplicationModel>
    {
        new ApplicationModel
        {
            EmployeeId = empId,
            Type = EnumTypes.ApplicationType.REMOTE,
            Name = "Remote Work",
            Note = "Remote Work Note",
            StartDate = new DateTime(year, 1, 1),
            EndDate = new DateTime(year, 1, 2),
            CreatedDate = new DateTime(year, 1, 1)
        }
    };

            await _context.ApplicationList.AddRangeAsync(approvedLeaveApplications);
            await _context.ApplicationList.AddRangeAsync(requestedOvertimeListInMonth);
            await _context.ApplicationList.AddRangeAsync(approvedOvertimeListInMonth);
            await _context.ApplicationList.AddRangeAsync(remoteApplicationList);
            await _context.SaveChangesAsync();

            // In ra dữ liệu mẫu để kiểm tra
            var allApplications = await _context.ApplicationList.AsNoTracking().ToListAsync();
            foreach (var app in allApplications)
            {
                Console.WriteLine($"Application in DB - ID: {app.Id}, EmployeeId: {app.EmployeeId}, Type: {app.Type}, Status: {app.Status}, CreatedDate: {app.CreatedDate}");
            }

            // Act
            var result = await _applicationService.GetYearlyStatistics(empId, year);

            // In ra kết quả trả về
            Console.WriteLine($"CountAnnualLeave: {result.CountAnnualLeave}");
            foreach (var detail in result.DetailLeaveApplicationList)
            {
                Console.WriteLine($"DetailLeaveApplication: {detail}");
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CountAnnualLeave);
            Assert.Single(result.DetailLeaveApplicationList);
            Assert.NotEmpty(result.DetailOvertimeApplicationList);
            Assert.Equal("9.00h/9.00h", result.Overtime.ToString(System.Globalization.CultureInfo.InvariantCulture).Replace(",","."));
            Assert.Equal(1, result.CountRemoteDays);
        }







        [Fact]
        public async Task GetYearlyStatistics_NoData_ShouldReturnEmptyStatistics()
        {
            await ResetDbContext();

            // Arrange
            var empId = 1L;
            var year = 2024;

            // Không thêm bất kỳ ứng dụng nào vào cơ sở dữ liệu trong bộ nhớ.

            // Act
            var result = await _applicationService.GetYearlyStatistics(empId, year);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.CountAnnualLeave);
            Assert.Empty(result.DetailLeaveApplicationList);
            Assert.Empty(result.DetailOvertimeApplicationList);
            // Sử dụng InvariantCulture để đảm bảo định dạng đúng
            Assert.Equal("0.00h/0.00h", result.Overtime.Replace(",", "."));
            Assert.Equal(0, result.CountRemoteDays);
        }


    }
}
