using MagicVilla_VillaApi.IRepository;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("api/UserAuth")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _user;
        private readonly APIResponse _response;
        public UserController(IUserRepository user)
        {
            _user = user;
            _response = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            var loginUser = await _user.Login(model);
            if (loginUser.User == null || loginUser.Token == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("User Name or Password is Incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = loginUser;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterationRequestDTO model)
        {
            bool uniqueUser = _user.isUniqueUser(model.UserName);
            if (uniqueUser)
            {
                _response.Result = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("User Already Exists");
                return BadRequest(_response);
            }
            var user = await _user.Register(model);
            if (user == null)
            {
                _response.Result = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }
            _response.Result = user;
            _response.StatusCode= HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
