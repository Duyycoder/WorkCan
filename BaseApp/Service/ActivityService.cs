using BaseApp.Constants;
using BaseApp.DTO;
using BaseApp.Helpers;
using BaseApp.Models;
using BaseApp.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BaseApp.Service
{
    public interface IActivityService
    {
        Task<List<ActivityModel>> GetAll();

        Task<ResponseActivitiesInDayDTO> GetAllActivitiesInDay(long empId);

        Task<ResponseAttendance> GetAttendanceInDay(long empId, int month, int year);
        
        Task<bool> CreateActivity(RequestActivityDTO requestActivityDTO);

        Task<bool> UpdateActivity();

        Task SetNoteForActivity(EnumTypes.ActivityType activityType, string note, long empId);

        Task<bool> DeleteActivity();
    }

    public class ActivityService : IActivityService
    {
        private readonly IRepositoryManager _repositoryManager;

        private readonly ILocationService _locationService;

        private readonly ICompanyInfoService _companyInfoService;

        public ActivityService(IRepositoryManager repositoryManager, ILocationService locationService, ICompanyInfoService companyInfoService)
        {
            _repositoryManager = repositoryManager;
            _locationService = locationService;
            _companyInfoService = companyInfoService;
        }

        public async Task<ActivityModel> GetEarliestCheckIn(long empId) 
        {

            ActivityModel earliestCheckIn = await _repositoryManager.activityRepostitory
                .FindByCondition(a => a.Location.EmpId == empId 
                && a.CreatedDate.Date.Equals(DateTime.Now.Date)
                && a.Type.Equals(EnumTypes.ActivityType.CHECKIN))
                .OrderBy(a => a.CreatedDate).Include(a => a.Location)
                .FirstOrDefaultAsync();

            return earliestCheckIn;
        }

        public async Task<ActivityModel> GetLatestCheckOut(long empId)
        {

            ActivityModel lastestCheckOut = await _repositoryManager.activityRepostitory
                .FindByCondition(a => a.Location.EmpId == empId 
                && a.CreatedDate.Date.Equals(DateTime.Now.Date) 
                && a.Type.Equals(EnumTypes.ActivityType.CHECKOUT))
                .OrderByDescending(a => a.CreatedDate).Include(a => a.Location)
                .FirstOrDefaultAsync();

            return lastestCheckOut;
        }

        public async Task<List<ActivityModel>> GetBreakListInDay(long empId, bool isBreakStart)
        {

            // if isBreakStart true , return break start list
            // else return break end list
            List<ActivityModel> breaklist = isBreakStart ? 
                await _repositoryManager.activityRepostitory
                .FindByCondition(a => a.Location.EmpId == empId 
                && a.CreatedDate.Date.Equals(DateTime.Now.Date)
                && a.Type.Equals(EnumTypes.ActivityType.BREAKSTART))
                .OrderBy(a => a.CreatedDate).Include(a => a.Location)
                .ToListAsync() : 
                await _repositoryManager.activityRepostitory
                .FindByCondition(a => a.Location.EmpId == empId
                && a.CreatedDate.Date.Equals(DateTime.Now.Date)
                && a.Type.Equals(EnumTypes.ActivityType.BREAKEND))
                .OrderBy(a => a.CreatedDate).Include(a => a.Location)
                .ToListAsync();

            return breaklist;

        }

        private async Task<ActivityModel> GetLastestActivityInDay(long empId, DateTime date)
        {

            ActivityModel result = await _repositoryManager.activityRepostitory
                .FindByCondition(a => a.CreatedDate.Date == date.Date
                && a.Location.EmpId == empId)
                .OrderByDescending(a => a.CreatedDate)
                .Include(a => a.ChildActivities)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> CreateActivity(RequestActivityDTO requestActivityDTO)
        {
            bool isActivityBreakEnd = false;
            ActivityModel lastestBreakStart = new ActivityModel();
            CompanyInfoModel companyInfo = await _companyInfoService.GetById(1);

            ActivityModel checkInToday = await _repositoryManager.activityRepostitory.FindByCondition(activity =>
                activity.Location.EmpId == requestActivityDTO.EmpId &&
                activity.CreatedDate.Date == DateTime.Now.Date
            ).AsNoTracking().FirstOrDefaultAsync();

            // check if request is not check in and if employee has checked in today
            if (!requestActivityDTO.Type.Equals(EnumTypes.ActivityType.CHECKIN) && checkInToday == null)
            {
                throw new Exception(Constants.DevMessageConstants.EMPLOYEE_HASNT_CHECKED_IN);
            }

            ActivityModel latestActivityIndDay = await GetLastestActivityInDay(requestActivityDTO.EmpId, DateTime.Now);

            if (latestActivityIndDay != null)
            {
                switch (requestActivityDTO.Type)
                {
                    case Constants.EnumTypes.ActivityType.CHECKIN:
                        {
                            if (latestActivityIndDay.Type.Equals(EnumTypes.ActivityType.CHECKIN))
                            {
                                throw new Exception("Employee has checked in already");
                            }
                            
                            if (latestActivityIndDay.Type.Equals(EnumTypes.ActivityType.CHECKOUT))
                            {
                                throw new Exception("Employee checked out so they can't check in anymore");
                            }

                            // check if employee is late
                            if (DateTime.Now.CompareTo(companyInfo.StartTime) == 1)
                            {
                                throw new Exception("Employee is late");
                            }
                            break;
                        }

                    case Constants.EnumTypes.ActivityType.CHECKOUT:
                        {

                            if (latestActivityIndDay.Type.Equals(EnumTypes.ActivityType.CHECKIN)) break;

                            else
                            {
                                if (latestActivityIndDay.Type.Equals(EnumTypes.ActivityType.BREAKSTART))
                                {
                                    throw new Exception("Employee hasnt break end yet");
                                }

                                if (latestActivityIndDay.Type.Equals(EnumTypes.ActivityType.CHECKOUT))
                                {
                                    throw new Exception("Employee checked out");
                                }
                            }

                            break;
                        }

                    case Constants.EnumTypes.ActivityType.BREAKSTART:
                        {

                            if (checkInToday == null)
                            {
                                throw new Exception(Constants.DevMessageConstants.EMPLOYEE_HASNT_CHECKED_IN);
                            }

                            if (latestActivityIndDay.Type.Equals(EnumTypes.ActivityType.CHECKOUT))
                            {
                                throw new Exception("Employee logged checkout");
                            }

                            if (latestActivityIndDay.Type.Equals(EnumTypes.ActivityType.BREAKSTART) && latestActivityIndDay.ChildActivities.Count == 0)
                            {
                                throw new Exception("Employee logged break start");
                            }

                            break;
                        }

                    case Constants.EnumTypes.ActivityType.BREAKEND:
                        {

                            ActivityModel breakStartToday = await _repositoryManager.activityRepostitory.FindByCondition(activity =>
                                activity.Location.EmpId == requestActivityDTO.EmpId &&
                                activity.CreatedDate.Date == DateTime.Now.Date &&
                                activity.Type == Constants.EnumTypes.ActivityType.BREAKSTART
                            ).AsNoTracking().OrderByDescending(acitivty => acitivty.CreatedDate)
                            .Include(a => a.ChildActivities).FirstOrDefaultAsync();

                            if (latestActivityIndDay.Type.Equals(EnumTypes.ActivityType.CHECKOUT))
                            {
                                throw new Exception("Employee logged checkout");
                            }

                            if (breakStartToday == null)
                            {
                                throw new Exception(Constants.DevMessageConstants.EMPLOYEE_HASNT_BREAK_START);
                            }

                            if (breakStartToday.ChildActivities.Count != 0)
                            {
                                throw new Exception("Break start already has break end");
                            }

                            lastestBreakStart = breakStartToday;

                            isActivityBreakEnd = true;

                            break;
                        }
                    default:
                        break;

                }
            }

            DateTimeHelper dateTimeHelper = new DateTimeHelper();

            try
            {
                var timeOnlyValue = dateTimeHelper.TryParseDateTimeOrTimeOnly(requestActivityDTO.ActivityTime);

                if (timeOnlyValue.TimeOnly.HasValue) 
                {
                    CalculateDistanceHelper calculateDistanceHelper = new CalculateDistanceHelper();

                    bool isUserInRangeOfCompanyLocation = false;

                    var companyInfoList = await _companyInfoService.GetAll();

                    // validates if login's location is in range of company location
                    companyInfoList.ForEach(companyInfo =>
                    {
                        double distanceFromRequestToCompany = calculateDistanceHelper.GetDistanceFromLatLonInMeters(companyInfo.latitude, companyInfo.longtitude, requestActivityDTO.Latitude, requestActivityDTO.Longtitude);
                        if (distanceFromRequestToCompany > 100)
                        {
                            throw new Exception("User is not at company");
                        }
                    });

                    ActivityModel newActivityModel = new ActivityModel
                    {
                        Type = requestActivityDTO.Type.Value,
                        ActivityTime = timeOnlyValue.TimeOnly.Value,
                    };

                    LocationModel existedLocationByEmpId = await _locationService.GetAllByEmpIdAndLatAndLon(requestActivityDTO.EmpId, requestActivityDTO.Latitude, requestActivityDTO.Longtitude);

                    if (existedLocationByEmpId == null)
                    {

                        LocationModel locationModel = new LocationModel
                        {
                            DeviceId = requestActivityDTO.DeviceId,
                            Description = "In Office",
                            Longtitude = requestActivityDTO.Longtitude,
                            Latitude = requestActivityDTO.Latitude,
                            EmpId = requestActivityDTO.EmpId,
                        };

                        await _locationService.CreateLocation(locationModel);

                        newActivityModel.LocationId = locationModel.Id;
                    }

                    else
                    {
                        newActivityModel.LocationId = existedLocationByEmpId.Id;
                    }

                    if (isActivityBreakEnd)
                    {
                        newActivityModel.ParentActivityId = lastestBreakStart.Id;
                        lastestBreakStart.ChildActivities.Add(newActivityModel);
                        _repositoryManager.activityRepostitory.Update(lastestBreakStart);
                    }

                    await _repositoryManager.activityRepostitory.Create(newActivityModel);

                    await _repositoryManager.SaveAsync();
                }
            }
            catch (FormatException ex)
            {
                return false;
            }


            return true;
        }

        public Task<bool> DeleteActivity()
        {
            throw new NotImplementedException();
        }

        public Task<List<ActivityModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateActivity()
        {
            throw new NotImplementedException();
        }

        public async Task SetNoteForActivity(EnumTypes.ActivityType activityType, string note, long empId)
        {

            // find lastest activity in day matchest with activity type and by location id
            ActivityModel activity = await _repositoryManager.activityRepostitory.FindByCondition(a => a.Type == activityType 
                                                                                                && a.CreatedDate.Date.Equals(DateTime.Now.Date)
                                                                                                && a.Location.EmpId == empId)
                                                                                                .OrderByDescending(a => a.CreatedDate)
                                                                                                .AsNoTracking().FirstOrDefaultAsync();
            if(activity == null)
            {
                throw new Exception("Employee havent logged activity in day");
            }

            if(string.IsNullOrEmpty(note))
            {
                throw new Exception("Note cannot be null");
            }

            if(activityType.Equals(EnumTypes.ActivityType.BREAKEND) && activity.ParentActivity == null)
            {
                throw new Exception("Employee didnt log break start before break end");
            }

            activity.Note = note;

            _repositoryManager.activityRepostitory.Update(activity);

            await _repositoryManager.SaveAsync();

        }

        public async Task<ResponseActivitiesInDayDTO> GetAllActivitiesInDay(long empId)
        {
            ActivityModel earliestCheckIn = await GetEarliestCheckIn(empId);
            ActivityModel latestCheckOut = await GetLatestCheckOut(empId);
            List<ActivityModel> breakStartList = await GetBreakListInDay(empId, true);
            List<ActivityModel> breakEndList = await GetBreakListInDay(empId, false);

            ResponseActivitiesInDayDTO result = new ResponseActivitiesInDayDTO();

            if (earliestCheckIn != null)
            {
                result.EarliestCheckIn = new ResponseActivity
                {
                    Id = earliestCheckIn.Id,
                    ActivityType = earliestCheckIn.Type,
                    ActivityTime = earliestCheckIn.ActivityTime,
                    Longtitude = earliestCheckIn.Location.Longtitude,
                    Latitude = earliestCheckIn.Location.Latitude,
                };
            }

            if (latestCheckOut != null)
            {
                result.LastestCheckOut = new ResponseActivity
                {
                    Id = latestCheckOut.Id,
                    ActivityType = latestCheckOut.Type,
                    ActivityTime = latestCheckOut.ActivityTime,
                    Longtitude = latestCheckOut.Location.Longtitude,
                    Latitude = latestCheckOut.Location.Latitude,
                };
            }

            if (breakStartList != null)
            {
                result.BreakStartList = breakStartList.Select(activity => new ResponseActivity
                {
                    Id = activity.Id,
                    ActivityType = activity.Type,
                    ActivityTime = activity.ActivityTime,
                    Longtitude = activity.Location.Longtitude,
                    Latitude = activity.Location.Latitude,
                }).ToList();
            }

            if (breakEndList != null)
            {
                result.BreakEndList = breakEndList.Select(activity => new ResponseActivity
                {
                    Id = activity.Id,
                    ActivityType = activity.Type,
                    ActivityTime = activity.ActivityTime,
                    Longtitude = activity.Location.Longtitude,
                    Latitude = activity.Location.Latitude,
                }).ToList();
            }

            return result;
        }

        public async Task<ResponseAttendance> GetAttendanceInDay(long empId, int month, int year)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            List<ResponseDailyLog> responseDailyLogList = new List<ResponseDailyLog>();
            CompanyInfoModel companyInfoModel = await _companyInfoService.GetById(1);
            TimeSpan totalWorkTime = new TimeSpan();
            TimeSpan totalBreakTime = new TimeSpan();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {

                List<string> dailyStatus = new List<string>();

                ResponseDailyLog responseDailyLog = new ResponseDailyLog();

                // get days in month
                CultureInfo culture = new CultureInfo("en-US");
                string dayOfWeek = culture.DateTimeFormat.GetDayName(date.DayOfWeek);
                responseDailyLog.Day = date.Day + " (" + dayOfWeek + ")";

                if (date > DateTime.Now)
                {
                    responseDailyLog.Status = "Not Check Out";
                }

                else
                {
                    ActivityModel earliestCheckIn = await _repositoryManager.activityRepostitory
                                                .FindByCondition(a => a.Type == Constants.EnumTypes.ActivityType.CHECKIN 
                                                && a.CreatedDate.Date == date.Date
                                                && a.Location.EmpId == empId)
                                                .OrderBy(a => a.CreatedDate).Include(a => a.Location)
                                                .FirstOrDefaultAsync();

                    ActivityModel lastestCheckOut = await _repositoryManager.activityRepostitory
                                                    .FindByCondition(a => a.Type == Constants.EnumTypes.ActivityType.CHECKOUT
                                                    && a.CreatedDate.Date == date.Date
                                                    && a.Location.EmpId == empId)
                                                    .OrderByDescending(a => a.CreatedDate).Include(a => a.Location)
                                                    .FirstOrDefaultAsync();

                    List<ActivityModel> breakStartList = await _repositoryManager.activityRepostitory
                                                    .FindByCondition(a => a.Type == Constants.EnumTypes.ActivityType.BREAKSTART
                                                    && a.CreatedDate.Date == date.Date
                                                    && a.Location.EmpId == empId)
                                                    .Include(a => a.ParentActivity)
                                                    .ToListAsync();

                    List<ActivityModel> breakEndList = await _repositoryManager.activityRepostitory
                                                    .FindByCondition(a => a.Type == Constants.EnumTypes.ActivityType.BREAKEND
                                                    && a.CreatedDate.Date == date.Date
                                                    && a.Location.EmpId == empId)
                                                    .Include(a => a.ParentActivity)
                                                    .ToListAsync();

                    if (earliestCheckIn != null && breakEndList != null)
                    {
                        TimeSpan totalBreakTimeInDay = new TimeSpan(0, 1, 0, 0);

                        foreach (ActivityModel breakEnd in breakEndList)
                        {
                            if (breakEnd.ParentActivity != null)
                            {
                                TimeSpan diffInBreak = breakEnd.ActivityTime - breakEnd.ParentActivity.ActivityTime;
                                totalBreakTimeInDay += diffInBreak;
                            }
                        }

                        TimeSpan totalWorkTimeInDay = new TimeSpan();

                        // if employee hasn't check out
                        // work time = company's default break start - earliest check in
                        TimeOnly checkOutTime = new TimeOnly();
                        if (lastestCheckOut == null)
                        {
                            checkOutTime = companyInfoModel.DefaultBreakStart;
                        }

                        else
                        {
                            checkOutTime = lastestCheckOut.ActivityTime;
                            responseDailyLog.CheckOutTime = lastestCheckOut.ActivityTime.ToString("HH:mm");
                        }

                        totalWorkTimeInDay = checkOutTime - earliestCheckIn.ActivityTime - totalBreakTimeInDay;

                        totalWorkTime += totalWorkTimeInDay;

                        totalBreakTime += totalBreakTimeInDay;

                        responseDailyLog.CheckInTime = earliestCheckIn.ActivityTime.ToString("HH:mm");

                        responseDailyLog.BreakTime = string.Format("{0:D2}:{1:D2}", totalBreakTimeInDay.Hours, totalBreakTimeInDay.Minutes);

                        responseDailyLog.WorkTime = string.Format("{0:D2} hours {1:D2} minutes", totalWorkTimeInDay.Hours, totalWorkTimeInDay.Minutes);
                 
                        if(earliestCheckIn.ActivityTime > companyInfoModel.StartTime)
                        {
                            dailyStatus.Add("Late");
                        }
                    }

                    if (date < DateTime.Now.Date)
                    {
                        if (lastestCheckOut != null)
                        {
                            dailyStatus.Add("Check Out");
                        }
                        else
                        {
                            dailyStatus.Add("Not Check Out");
                        }
                    }

                    else if (date == DateTime.Now.Date)
                    {
                        if (breakStartList.Count > breakEndList.Count)
                        {
                            dailyStatus.Add("Out of office");
                        }

                        if (lastestCheckOut != null)
                        {
                            dailyStatus.Add("Check Out");
                        }

                        else
                        {
                            dailyStatus.Add("Not Check Out");
                        }
                    }

                    if (dailyStatus.Count > 0)
                    {
                        responseDailyLog.Status = responseDailyLog.Status + dailyStatus[0];
                        if (dailyStatus.Count > 1)
                        {
                            for (int i = 1; i < dailyStatus.Count; i++)
                            {
                                responseDailyLog.Status += (" - " + dailyStatus[i]);
                            }
                        }
                    }
                }

                responseDailyLogList.Add(responseDailyLog);
            }

            // calculate request overtime and approved overtime
            List<ApplicationModel> overtimeApplicationList = await _repositoryManager.applicationRepository.FindByCondition(
                                                     a => a.Type.Equals(EnumTypes.ApplicationType.OVERTIME)
                                                     && a.CreatedDate.Month == month
                                                     && a.EmployeeId == empId)
                                                    .ToListAsync();

            TimeSpan totalRequestedOvertime = new TimeSpan();
            TimeSpan totalApprovedOvertime = new TimeSpan();
            TimeSpan totalNightApprovedOvertime = new TimeSpan();

            overtimeApplicationList.ForEach(a =>
            {
                
                switch (a.Status)
                {
                    case EnumTypes.ApplicationStatus.REQUESTED:
                        {
                            totalRequestedOvertime += (a.EndDate.Subtract(a.StartDate));
                            break;
                        }
                    case EnumTypes.ApplicationStatus.APPROVED:
                        {
                            // fixed overnight time
                            DateTime startOverNightTime = new DateTime(a.StartDate.Year, a.StartDate.Month, a.StartDate.Day, 22, 00, 00);
                            DateTime endOverNightTime = new DateTime(a.StartDate.Year, a.StartDate.Month, a.StartDate.Day, 6, 00, 00).AddDays(1);

                            if(a.EndDate < startOverNightTime)
                            {
                                totalNightApprovedOvertime = TimeSpan.Zero;
                            }

                            else
                            {
                                if (a.StartDate < startOverNightTime)
                                {
                                    if (a.EndDate < endOverNightTime)
                                    {
                                        totalNightApprovedOvertime += a.EndDate - startOverNightTime;
                                    }
                                    else
                                    {
                                        totalNightApprovedOvertime += endOverNightTime - startOverNightTime;
                                    }
                                }

                                else
                                {
                                    if (a.EndDate < endOverNightTime)
                                    {
                                        totalNightApprovedOvertime += a.EndDate - a.StartDate;
                                    }
                                    else
                                    {
                                        totalNightApprovedOvertime += endOverNightTime - a.StartDate;
                                    }
                                }
                            }

                            totalApprovedOvertime += (a.EndDate - a.StartDate);
                            break;
                        }
                }
            });

            totalWorkTime += totalApprovedOvertime;

            //calculate approved holiday working time
            List<ApplicationModel> holidayWorklist = await _repositoryManager.applicationRepository.FindByCondition(
                                                    a => a.Type.Equals(EnumTypes.ApplicationType.HOLIDAY)
                                                    && a.CreatedDate.Month == month
                                                    && a.EmployeeId == empId)
                                                   .ToListAsync();

            TimeSpan totalHolidayWorkTime = new TimeSpan();
            holidayWorklist.ForEach(a =>
            {
                switch (a.Status)
                {
                    case EnumTypes.ApplicationStatus.APPROVED:
                        {
                            totalHolidayWorkTime += (a.EndDate.Subtract(a.StartDate));
                            break;
                        }
                }
            });

            return new ResponseAttendance
            {
                TotalWorktime = (int)totalWorkTime.TotalHours + " hours " + totalWorkTime.Minutes + " minutes",
                TotalApprovedOvertime = (int)totalApprovedOvertime.TotalHours + " hours " + totalApprovedOvertime.Minutes + " minutes",
                Break = (int)totalBreakTime.TotalHours + " hours " + totalBreakTime.Minutes + " minutes",
                ResponseDailyLogs = responseDailyLogList,
                TotalPendingOvertime = (int)totalRequestedOvertime.TotalHours + " hours " + totalRequestedOvertime.Minutes + " minutes",
                TotalApprovedHolidayWork = (int)totalHolidayWorkTime.TotalHours + " hours " + totalHolidayWorkTime.Minutes + " minutes",
                TotalNightApprovedOvertime = (int)totalNightApprovedOvertime.TotalHours + " hours " + totalNightApprovedOvertime.Minutes + " minutes"
            };
        }

    }
}
