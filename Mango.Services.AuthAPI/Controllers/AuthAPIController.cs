using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMeassage = await _authService.Register(model);
            if (!String.IsNullOrEmpty(errorMeassage))
            {
                _response.IsSuccess = false;
                _response.Messagge = errorMeassage;
                return BadRequest(_response);

            }
            return Ok(_response);

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse == null)
            {
                {
                    _response.IsSuccess = false;
                    _response.Messagge = "Username or password is incorrect";
                    return BadRequest(_response);
                }
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }
    }
}
