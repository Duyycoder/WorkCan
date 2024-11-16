using Azure;
using BaseApp.Constants;
using BaseApp.DTO;
using BaseApp.Helpers;
using BaseApp.Models;
using BaseApp.Repository;
using BaseApp.Repository.Base;
using Microsoft.EntityFrameworkCore;
using static BaseApp.DTO.ResponseApplicationDTO;

namespace BaseApp.Service
{
    public interface IApplicationService
    {
        Task<bool> CreateApplication(RequestApplicationDTO requestApplicationDTO);

        Task<ResponseApplicationDTO> GetApplicationList(long empId, EnumTypes.ApplicationType applicationType, int page, int size, DateTime from, DateTime to);

        Task UpdateApplicationStatus(RequestUpdateApplicationStatusDTO requestUpdateApplicationStatusDTO);

        Task<ResponseYearlyStatisticsDTO> GetYearlyStatistics(long empId, int year);
    }

    public class ApplicationService : IApplicationService
    {

        private readonly IRepositoryManager _repositoryManager;

        public ApplicationService (IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<bool> CreateApplication(RequestApplicationDTO requestApplicationDTO)
        {
            ApplicationModel newApplicationModel = new ApplicationModel
            {
                Name = requestApplicationDTO.Title,
                Type = requestApplicationDTO.ApplicationType,
                StartDate = requestApplicationDTO.StartedDate,
                EndDate = requestApplicationDTO.EndedDate,
                Status = Constants.EnumTypes.ApplicationStatus.REQUESTED,
                EmployeeId = requestApplicationDTO.EmpId,
                Note = requestApplicationDTO.Note,
            };

            try
            {
                await _repositoryManager.applicationRepository.Create(newApplicationModel);
                await _repositoryManager.SaveAsync();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<ResponseApplicationDTO> GetApplicationList(long empId, 
                                                EnumTypes.ApplicationType applicationType, 
                                                int page, int size, DateTime from, DateTime to)
        {
            page = page <= 0 ? CommonConstants.DEFAULT_PAGE : page;
            size = size <= 0 ? CommonConstants.SIZE_OF_PAGE : size;

            var query = _repositoryManager.applicationRepository.FindAll().AsQueryable();

            query = query.Where(app => app.EmployeeId == empId && app.Type.Equals(applicationType));

            // Filter by time
            if (from != DateTime.MinValue && to != DateTime.MinValue)
            {
                query = query.Where(app => app.StartDate >= from && app.EndDate <= to);
            }

            // Paginate and sorting
            var paginatedResult = await query.OrderByDescending(app => app.Id)
                                             .ToPaginatedResponseAsync(page, size);

            // Handle result's data
            ResponseApplicationDTO responseApplicationDTO = new ResponseApplicationDTO();
            if (paginatedResult.Items != null)
            {
                List<SingleResponseApplicationDTO> results = new List<SingleResponseApplicationDTO>();
                foreach (ApplicationModel application in paginatedResult.Items)
                {
                    SingleResponseApplicationDTO singleResponseApplicationDTO = new SingleResponseApplicationDTO
                    {
                        Id = application.Id,
                        Title = application.Name,
                        Reason = application.Note,
                        Type = application.Type,
                        StartedDate = application.StartDate.ToString("dd/MM/yyyy HH:mm"),
                        EndedDate = application.EndDate.ToString("dd/MM/yyyy HH:mm"),
                        Status = application.Status,
                        CreatedDate = application.CreatedDate.ToString("dd/MM/yyyy HH:mm")
                    };

                    results.Add(singleResponseApplicationDTO);
                }

                responseApplicationDTO.ApplicationList = results;
            }

            responseApplicationDTO.PageNumber = paginatedResult.PageNumber;
            responseApplicationDTO.PageSize = paginatedResult.PageSize;
            responseApplicationDTO.TotalRecords = paginatedResult.TotalRecords;
            responseApplicationDTO.TotalPages = paginatedResult.TotalPages;

            return responseApplicationDTO;
        }


        public async Task UpdateApplicationStatus(RequestUpdateApplicationStatusDTO requestUpdateApplicationStatusDTO)
        {
            ApplicationModel applicationModel = await _repositoryManager.applicationRepository.FindByCondition(a => a.Id == requestUpdateApplicationStatusDTO.ApplicationId).FirstOrDefaultAsync();

            if(applicationModel == null)
            {
                throw new Exception(DevMessageConstants.OBJECT_IS_EMPTY);
            }

            if((applicationModel.Status.Equals(EnumTypes.ApplicationStatus.REJECTED)
                || applicationModel.Status.Equals(EnumTypes.ApplicationStatus.APPROVED)
                && requestUpdateApplicationStatusDTO.ApplicationStatus.Equals(EnumTypes.ApplicationStatus.REQUESTED)))
            {
                throw new Exception("Employee can't request approved or requested application");
            }

            applicationModel.Status = requestUpdateApplicationStatusDTO.ApplicationStatus;

            _repositoryManager.applicationRepository.Update(applicationModel);

            await _repositoryManager.SaveAsync();

        }

        public async Task<ResponseYearlyStatisticsDTO> GetYearlyStatistics(long empId, int year)
        {
            ResponseYearlyStatisticsDTO responseYearlyStatisticsDTO = new ResponseYearlyStatisticsDTO();    

            // calculate annual leave
            List<ApplicationModel> approvedLeaveApplications = await _repositoryManager.applicationRepository
                                                        .FindByCondition(a => a.EmployeeId == empId
                                                        && a.Type.Equals(EnumTypes.ApplicationType.LEAVE)
                                                        && a.Status.Equals(EnumTypes.ApplicationStatus.APPROVED)
                                                        && a.CreatedDate.Year == year)
                                                        .ToListAsync();

            if(approvedLeaveApplications != null)
            {
                responseYearlyStatisticsDTO.CountAnnualLeave = approvedLeaveApplications.Count;
                responseYearlyStatisticsDTO.DetailLeaveApplicationList = new List<string>();
                foreach (ApplicationModel item in approvedLeaveApplications)
                {
                    // if employee leaves in a same day , return "1 Day"
                    string totalLeaveDay = (item.EndDate - item.StartDate).TotalDays < 1 ? (1.ToString() + " Day") : ((item.EndDate - item.StartDate).TotalDays.ToString() + " Days");

                    string apporvedLeaveResponse = item.StartDate.ToString("yyyy/MM/dd")
                                                    + " - " + item.EndDate.ToString("yyyy/MM/dd")
                                                    + " - " + totalLeaveDay;

                    responseYearlyStatisticsDTO.DetailLeaveApplicationList.Add(apporvedLeaveResponse);
                }
            }

            double requestedOvertimeInYear = 0;
            double approvedOvertimeInYear = 0;

            responseYearlyStatisticsDTO.DetailOvertimeApplicationList = new List<string>();

            // display overtime
            for (int month = 1; month <= 12; month++)
            {

                List<ApplicationModel> requestedOvertimeListInMonth = await _repositoryManager.applicationRepository
                                                                            .FindByCondition(e => e.CreatedDate.Month == month
                                                                            && e.Type.Equals(EnumTypes.ApplicationType.OVERTIME)
                                                                            && e.Status.Equals(EnumTypes.ApplicationStatus.REQUESTED))
                                                                            .ToListAsync();

                List<ApplicationModel> approvedOvertimeListInMonth = await _repositoryManager.applicationRepository
                                                                           .FindByCondition(e => e.CreatedDate.Month == month
                                                                           && e.Type.Equals(EnumTypes.ApplicationType.OVERTIME)
                                                                           && e.Status.Equals(EnumTypes.ApplicationStatus.APPROVED))
                                                                           .ToListAsync();

                if(requestedOvertimeListInMonth.Count != 0 && approvedOvertimeListInMonth.Count != 0)
                {
                    TimeSpan totalRequestedOvertime = new TimeSpan();
                    TimeSpan totalApprovedOvertime = new TimeSpan();
                    requestedOvertimeListInMonth.ForEach(a => totalRequestedOvertime += (a.EndDate - a.StartDate));
                    approvedOvertimeListInMonth.ForEach(a => totalApprovedOvertime += (a.EndDate - a.StartDate));
                    responseYearlyStatisticsDTO.DetailOvertimeApplicationList.Add(year + "/" + month + " " + totalRequestedOvertime.TotalHours.ToString("0.00") + "h" + "/" + totalApprovedOvertime.TotalHours.ToString("0.00") + "h");
                    requestedOvertimeInYear += totalRequestedOvertime.TotalHours;
                    approvedOvertimeInYear += totalApprovedOvertime.TotalHours;
                }
            }

            responseYearlyStatisticsDTO.Overtime = requestedOvertimeInYear.ToString("0.00") + "h/" + approvedOvertimeInYear.ToString("0.00") + "h";

            List<ApplicationModel> remoteApplicationList = await _repositoryManager.applicationRepository.FindByCondition(a => a.EmployeeId == empId
                                                                                                    && a.CreatedDate.Year == year
                                                                                                    && a.Type.Equals(EnumTypes.ApplicationType.REMOTE))
                                                                                                    .ToListAsync();
            if(remoteApplicationList.Count != 0)
            {
                TimeSpan totalRemoteDay = new TimeSpan();

                remoteApplicationList.ForEach(a => totalRemoteDay += (a.EndDate - a.StartDate));

                responseYearlyStatisticsDTO.CountRemoteDays = totalRemoteDay.Days;
            }

            return responseYearlyStatisticsDTO;
        }

    }
}
