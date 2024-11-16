using BaseApp.Constants;
using BaseApp.DTO;
using BaseApp.Models;
using BaseApp.Service;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaseApp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly IEmployeeService _empService;

        private readonly ITokenService _tokenService;

        public EmployeeController(IEmployeeService empService, ITokenService tokenService)
        { 
            _empService = empService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseDataDTO<TokenResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromQuery] bool isRememberMeActive, [FromBody] LogInDTO loginParam)
        {

            if (string.IsNullOrEmpty(loginParam.DeviceId)) 
            {
                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = "User dont have an device's id"
                });
            }
            try
            {
                var authenRes = await _tokenService.Login(loginParam.EmpId, loginParam.Password, isRememberMeActive, loginParam.Longtitude, loginParam.Latitude, loginParam.DeviceId);

                if (authenRes is null)
                {
                    return BadRequest(new ResponseDataDTO<string>
                    {
                        Succeed = false,
                        Code = StatusCodes.Status400BadRequest,
                        ErrorMessage = DevMessageConstants.OBJECT_IS_EMPTY
                    }); ;
                }

                return Ok(new ResponseDataDTO<TokenResponse>
                {
                    Data = authenRes,
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
                    ErrorMessage = ex.Message
                });
            }

        }

        [HttpPost("change-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] RequestChangePasswordDTO requestChangePasswordDTO)
        {
            try
            {
                bool flag = await _empService.UpdatePassword(requestChangePasswordDTO.EmpId, requestChangePasswordDTO.Password);

                if (!flag)
                {
                    return BadRequest(new ResponseDataDTO<string>
                    {
                        Data = DevMessageConstants.NOTIFICATION_UPDATE_FAILED,
                        Succeed = false,
                        Code = StatusCodes.Status400BadRequest
                    });
                }

                return Ok(new ResponseDataDTO<string>
                {
                    Data = DevMessageConstants.NOTIFICATION_UPDATE_SUCCESS,
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
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        public IActionResult Logout()
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (accessToken != null)
            {
                // set cookie expiration to the past
                Response.Cookies.Append("AccessToken", "", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true, // not accessible through JS
                    Secure = true, // only sent over HTTPs
                    SameSite = SameSiteMode.Strict // prevent cross-site cookie sharing
                });
                return Ok(new { message = "Successfully logged out and token cookie deleted." });
            }

            return Unauthorized(new { message = "You haven't logged in" });
        }

        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseDataDTO<PaginatedResultDTO<EmployeeModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<PaginatedResultDTO<string>>), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllUser([FromQuery] int page, int size)
        {
            var result = await _empService.GetAll();

            if (!result.Any())
            {
                return Ok(new ResponseDataDTO<string>
                {
                    Data = DevMessageConstants.OBJECT_IS_EMPTY,
                    Succeed = true,
                    Code = StatusCodes.Status204NoContent
                });
            }

            page = page <= 0 ? CommonConstants.DEFAULT_PAGE : page;
            size = size <= 0 ? CommonConstants.SIZE_OF_PAGE : size;

            if (page == 1 && size >= result.Count)
            {
                return Ok(new ResponseDataDTO<List<EmployeeModel>>
                {
                    Data = result,
                    Succeed = true,
                    Code = StatusCodes.Status200OK
                });
            }

            var paginatedResult = new PaginatedResultDTO<EmployeeModel>
            {
                CurrentPage = page,
                ObjectList = result.Skip((page - 1) * size).Take(size).ToList(),
                PageSize = size,
                TotalCount = result.Count
            };

            return Ok(new ResponseDataDTO<PaginatedResultDTO<EmployeeModel>>
            {
                Data = paginatedResult,
                Succeed = true,
                Code = StatusCodes.Status200OK
            });
        }

        [HttpGet("{empId}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseDataDTO<PaginatedResultDTO<EmployeeModel>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<PaginatedResultDTO<string>>), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetUserById([FromRoute] long empId)
        {
            var result = await _empService.GetEmpProfile(empId);

            if (result == null)
            {
                return Ok(new ResponseDataDTO<string>
                {
                    Data = DevMessageConstants.OBJECT_IS_EMPTY,
                    Succeed = true,
                    Code = StatusCodes.Status204NoContent
             
                });
            }
            return Ok(new ResponseDataDTO<ResponseEmpDetailDTO>
            {
                Data = result,
                Succeed = true,
                Code = StatusCodes.Status200OK
            });
        }

        [HttpPost("")]
        [Authorize(Policy = "AdminPolicy")]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateUser([FromForm] RequestEmployeeDTO employee)
        {

            var adminId = User.FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(adminId))
            {
                return Forbid(DevMessageConstants.FORBIDEN_CONTENT);
            }

            // if request is invalid
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .Select(ms => new
                {
                    Field = ms.Key,
                    Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                })
                .ToString();

                return BadRequest(new ResponseDataDTO<string>
                {
                    Data = DevMessageConstants.ADD_OBJECT_FAILED,
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = errors
                });
            }
            try
            {
                bool flag = await _empService.Create(employee);
                if (!flag)
                {
                    return BadRequest(new ResponseDataDTO<string>
                    {
                        Data = DevMessageConstants.ADD_OBJECT_FAILED,
                        Succeed = true,
                        Code = StatusCodes.Status400BadRequest
                    });
                }
                return Ok(new ResponseDataDTO<string>
                {
                    Data = DevMessageConstants.ADD_OBJECT_SUCCESS,
                    Succeed = true,
                    Code = StatusCodes.Status200OK
                });
            }
            catch (Exception ex)
            {
                List<string> errorList = new List<string> { ex.Message };

                return BadRequest(new ResponseDataDTO<string>
                {
                    Data = DevMessageConstants.ADD_OBJECT_FAILED,
                    Succeed = true,
                    Code = StatusCodes.Status400BadRequest
                });
            }
            return Ok(new ResponseDataDTO<string>
            {
                Data = DevMessageConstants.ADD_OBJECT_SUCCESS,
                Succeed = true,
                Code = StatusCodes.Status200OK
            });
        }

        [HttpPut("{empId}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateEmployee([FromRoute] long empId,[FromForm] RequestEmployeeDTO employee)
        {

            // if request is invalid
            if (!ModelState.IsValid)
            {
                var error = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .Select(ms => new
                {
                    Field = ms.Key,
                    Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                }).ToString();

                return BadRequest(new ResponseDataDTO<string>
                {
                    Data = DevMessageConstants.NOTIFICATION_UPDATE_FAILED,
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = error
                });
            }

            try
            {
                bool flag = await _empService.Update(empId, employee);
                if (!flag)
                {
                    return BadRequest(new ResponseDataDTO<string>
                    {
                        Succeed = false,
                        Code = StatusCodes.Status400BadRequest,
                        ErrorMessage = DevMessageConstants.NOTIFICATION_UPDATE_FAILED
                    });
                }
                return Ok(new ResponseDataDTO<string>
                {
                    Data = DevMessageConstants.NOTIFICATION_UPDATE_SUCCESS,
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
                    ErrorMessage = ex.Message
                });
            }

        }

        [HttpDelete("{empId}")]
        [Authorize(Policy = "AdminPolicy")]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEmployee([FromRoute] long empId)
        {
            // if request is invalid
            if (!ModelState.IsValid)
            {
                
                // get errors in fluent validator
                var errors = ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .Select(ms => new
                {
                    Field = ms.Key,
                    Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                })
                .ToList();

                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = DevMessageConstants.NOTIFICATION_DELETE_FAILED
                });
            }

            // if request is valid
            bool flag = await _empService.Delete(empId);
            if(!flag)
            {
                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = true,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = DevMessageConstants.NOTIFICATION_DELETE_FAILED
                });
            }
            return Ok(new ResponseDataDTO<string>
            {
                Data = DevMessageConstants.NOTIFICATION_DELETE_SUCCESS,
                Succeed = true,
                Code = StatusCodes.Status200OK
            });
        }

    }
}
