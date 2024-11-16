using BaseApp.Constants;
using BaseApp.DTO;
using BaseApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BaseApp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class ApplicationController : ControllerBase
    {

        private IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Create new application
        /// </summary>
        /// <param name="applicationDTO">Application's required information: 0 - </param>
        /// <returns>Add object sucessfully string</returns>
        /// <response code="200">Add object sucessfully string</response>
        /// <response code="400">Invalid Request Application DTO or applicationId</response>
        /// <response code="401">Employee hasn't logged in yet</response>
        [HttpPost("")]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateApplication([FromBody] RequestApplicationDTO applicationDTO)
        {
            try
            {
                bool result = await _applicationService.CreateApplication(applicationDTO);
                if (result)
                {
                    return Ok(new ResponseDataDTO<string>
                    {
                        Data = Constants.DevMessageConstants.ADD_OBJECT_SUCCESS,
                        Succeed = true,
                        Code = StatusCodes.Status200OK,
                    });
                }
                return BadRequest(new ResponseDataDTO<string>
                {
                    Data = Constants.DevMessageConstants.ADD_OBJECT_FAILED,
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
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
        /// Get employee's application list
        /// </summary>
        /// <param name="empId">Emp's id</param>
        /// <param name="type">Application's Type</param>
        /// <param name="from">Application's start date min exclusive</param>
        /// <param name="to">Application's end date max exclusive</param>
        /// <param name="page">Current page of paginated list</param>
        /// <param name="size">Current size of paginated list</param>
        /// <returns>Application list</returns>
        /// <response code="200">Application list</response>
        /// <response code="401">Employee hasn't logged in yet</response>
        [HttpGet("{empId}")]
        [ProducesResponseType(typeof(ResponseDataDTO<ResponseApplicationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetApplicationList([FromRoute] long empId, [FromQuery] EnumTypes.ApplicationType type, int page, int size, DateTime from, DateTime to)
        {
            ResponseApplicationDTO result = await _applicationService.GetApplicationList(empId, type, page, size, from, to);
            if (result != null)
            {
                return Ok(new ResponseDataDTO<ResponseApplicationDTO>
                {
                    Data = result,
                    Succeed = true,
                    Code = StatusCodes.Status200OK,
                });
            }
            return Ok(new ResponseDataDTO<string>
            {
                Data = Constants.DevMessageConstants.OBJECT_IS_EMPTY,
                Succeed = true,
                Code = StatusCodes.Status200OK
            });
        }

        /// <summary>
        /// Update application stauts
        /// </summary>
        /// <param name="requestUpdateApplicationStatusDTO">Application's Required Information</param>
        /// <returns>Update application successfully string</returns>
        /// <response code="200">Update application successfully string</response>
        /// <response code="400">Employee can't request approved or requested application, Cant find Application, Wrong Application DTO</response>
        /// <response code="401">Employee hasn't logged in yet</response>
        [HttpPut("{empId}")]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateApplicationStatus([FromBody] RequestUpdateApplicationStatusDTO requestUpdateApplicationStatusDTO)
        {
            try
            {
                await _applicationService.UpdateApplicationStatus(requestUpdateApplicationStatusDTO);
                return Ok(new ResponseDataDTO<string>
                {
                    Data = Constants.DevMessageConstants.NOTIFICATION_UPDATE_SUCCESS,
                    Succeed = true,
                    Code = StatusCodes.Status200OK
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = ex.Message,
                });
            }
            
        }

        /// <summary>
        /// Get yearly statistics
        /// </summary>
        /// <param name="empId">Employee's ID</param>
        /// <param name="year">Current year</param>
        /// <returns>Yearly statistics</returns>
        /// <response code="200">Yearly statistics</response>
        /// <response code="401">Employee hasn't logged in yet</response>
        [HttpGet("get-yearly/{empId}")]
        [ProducesResponseType(typeof(ResponseDataDTO<ResponseYearlyStatisticsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetYearlyStatistics([FromRoute] long empId, [FromQuery] int year)
        {
            ResponseYearlyStatisticsDTO result = await _applicationService.GetYearlyStatistics(empId, year);  
            if(result != null)
            {
                return Ok(new ResponseDataDTO<ResponseYearlyStatisticsDTO>
                {
                    Data = result,
                    Code = StatusCodes.Status200OK,
                    Succeed = true,
                });
            }
            return Ok(new ResponseDataDTO<string>
            {
                Data = DevMessageConstants.OBJECT_IS_EMPTY,
                Code = StatusCodes.Status200OK,
                Succeed = true,
            });
        }

    }
}
