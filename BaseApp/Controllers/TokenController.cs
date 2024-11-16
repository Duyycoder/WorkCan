using BaseApp.Constants;
using BaseApp.Data;
using BaseApp.DTO;
using BaseApp.Models;
using BaseApp.Service;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static IdentityModel.OidcConstants;
using TokenResponse = BaseApp.DTO.TokenResponse;

namespace BaseApp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TokenController : ControllerBase
    {

        private readonly BaseAppDBContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly IEmployeeService _employeeService;

        public TokenController(BaseAppDBContext dbContext, ITokenService tokenService, IEmployeeService employeeService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="isRememberMeActive">If "Remember me" check box is checked id</param>
        /// <param name="refreshTokenRequest">Accesstoken string</param>
        /// <returns>New accesstoken and its expiry time</returns>
        /// <response code="200">Update employee successfully</response>
        /// <response code="400">Refresh token expires or Invalid refresh token</response>
        /// <response code="401">Employee hasn't logged in yet</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ResponseDataDTO<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDataDTO<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromQuery] bool isRememberMeActive, [FromBody] RequestRefreshTokenDTO refreshTokenRequest)
        {
            try
            {
                var tokenResponse = await _tokenService.RefreshToken(refreshTokenRequest.RefreshToken, isRememberMeActive);
                return Ok(new ResponseDataDTO<TokenResponse>
                {
                    Data = tokenResponse,
                    Succeed = true,
                    Code = StatusCodes.Status200OK
                });
            } 
            catch (Exception ex)
            {
                if (ex.Message.Equals(DevMessageConstants.TOKEN_EXPIRED))
                {
                    return Unauthorized(new ResponseDataDTO<string>
                    {
                        Succeed = false,
                        Code = StatusCodes.Status401Unauthorized,
                        ErrorMessage = ex.Message
                    });
                }
                return BadRequest(new ResponseDataDTO<string>
                {
                    Succeed = false,
                    Code = StatusCodes.Status400BadRequest,
                    ErrorMessage = ex.Message
                });
            }
            
        }
    }
}
