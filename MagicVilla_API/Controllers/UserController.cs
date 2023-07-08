using AutoMapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOS;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/v{version:apiVersion}/UsersAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UserController : ControllerBase
    {
        #region UserController Depandency Injection
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private APIResponse _response;
        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _response = new();
        }
        #endregion

        #region login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResponse = await _userRepository.Login(loginDto);

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { "Username or password is incorrect" };
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.Results = loginResponse;
            return Ok(_response);
        }
        #endregion

        #region register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationDto registerationDto)
        {
            bool isUnique = _userRepository.IsUnique(registerationDto.Username);

            if (!isUnique)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { "Username already exists" };
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var user = await _userRepository.Register(registerationDto);

            if (user == null)
            {
                _response.IsSuccess = false;
                _response.ErrorsMessages = new List<string>() { "Error while registering" };
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.Created;
            return Ok(_response);
        }
        #endregion
    }
}
