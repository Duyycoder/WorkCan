using BaseApp.Constants;
using BaseApp.DTO;
using BaseApp.Models;
using BaseApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BaseApp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class ActivityController : ControllerBase
    {

        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        /// <summary>
        /// Create new activtiy
        /// </summary>
        /// <param name="activityDTO">Acitvity's required information</param>
        /// <returns>Add object successfully string</returns>
        /// <response code="200">Add object successfully string</response>
        /// <response code="400">Invalid acitvity type....</response>
        /// <response code="401">Employee hasn't logged in yet</response>
        [HttpPost("")]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateActivity([FromBody] RequestActivityDTO activityDTO)
        {
            if (!activityDTO.Type.HasValue || !Enum.IsDefined(typeof(EnumTypes.ActivityType), activityDTO.Type.Value))
            {
                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = String.Format($"Invalid activity type: {activityDTO.Type.Value}")
                });
            }

            bool isCreatedActivity = false;
            try
            {
                isCreatedActivity = await _activityService.CreateActivity(activityDTO);
            }
            catch (Exception ex)
            {

                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = ex.Message
                });
            }

            if (!isCreatedActivity)
            {

                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = DevMessageConstants.ADD_OBJECT_FAILED
                });
            }

            return Ok(new ResponseDataDTO<string>
            {
                Data = Constants.DevMessageConstants.ADD_OBJECT_SUCCESS,
                Succeed = true,
                Code = StatusCodes.Status200OK
            });    
        }

        /// <summary>
        /// Get daily activity
        /// </summary>
        /// <param name="empId">Employee's id</param>
        /// <returns>Acivity in day list</returns>
        /// <response code="200">Acivity in day list</response>
        /// <response code="400">Cant find employee with given id</response>
        /// <response code="401">Employee hasn't logged in yet</response>
        [ProducesResponseType(typeof(ResponseDataDTO<ResponseActivitiesInDayDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-daily/{empId}")]
        public async Task<IActionResult> GetAllActivitiesInDay([FromRoute] long empId)
        {
            try
            {
                ResponseActivitiesInDayDTO result = await _activityService.GetAllActivitiesInDay(empId);

                return Ok(new ResponseDataDTO<ResponseActivitiesInDayDTO>
                {
                    Data = result,
                    Succeed = true,
                    Code = StatusCodes.Status200OK,
                });
            } 
            catch (Exception ex)
            {
                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = ex.Message
                });
            }

        }

        /// <summary>
        /// Update note for lastest activity type
        /// </summary>
        /// <param name="empId">Employee's id</param>
        /// <param name="requestActivity"> Activity's required information</param>
        /// <returns>Update object successfully string</returns>
        /// <response code="200">Update object successfully string</response>
        /// <response code="400">Update fail</response>
        /// <response code="401">Employee hasn't logged in yet</response>
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        [HttpPut("send-note/{empId}")]
        public async Task<IActionResult> SendNoteToActivity([FromRoute] long empId,[FromBody] RequestSendNoteDTO requestActivity)
        {

            if (!requestActivity.Type.HasValue || !Enum.IsDefined(typeof(EnumTypes.ActivityType), requestActivity.Type.Value))
            {
                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = String.Format($"Invalid activity type: {requestActivity.Type.Value}")
                });
            }

            try
            {
                await _activityService.SetNoteForActivity(requestActivity.Type.Value, requestActivity.Note, empId);
            } catch (Exception ex)
            {
                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = ex.Message
                });
            }

            return Ok(new ResponseDataDTO<string>
            {
                Data = DevMessageConstants.NOTIFICATION_UPDATE_SUCCESS,
                Succeed = true,
                Code = StatusCodes.Status200OK,
            });

        }

        /// <summary>
        /// Get daily logs in month
        /// </summary>
        /// <param name="empId">Employee's id</param>
        /// <param name="month">Employee's input with month</param>
        /// <param name="year">Employee's input with year</param>
        /// <returns>Daily acitvity list in month and statistics</returns>
        /// <response code="200">Daily acitvity list in month and statistics</response>
        /// <response code="401">Employee hasn't logged in yet</response>
        [ProducesResponseType(typeof(ResponseDataDTO<ResponseAttendance>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        [HttpGet("get-in-month")]
        public async Task<IActionResult> GetDailyLogs([FromQuery] long empId, int month, int year)
        {
            ResponseAttendance responseDaiyLog = await _activityService.GetAttendanceInDay(empId, month, year);

            return Ok(new ResponseDataDTO<ResponseAttendance>
            {
                Data = responseDaiyLog,
                Succeed = true,
                Code = StatusCodes.Status200OK,
            });

        }

    }
}
