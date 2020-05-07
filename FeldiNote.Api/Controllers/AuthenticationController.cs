using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FeldiNote.Api.Models;
using FeldiNote.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeldiNote.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public AuthenticationController(AuthenticationService authenticationService, IMapper mapper)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody]UserRegister model)
        {
            var user = _mapper.Map<User>(model);

            try
            {
                _authenticationService.Register(user, model.Password);
            }
            catch (Exception e)
            {
                return BadRequest($"Can't register the user : {e.Message}");
            }

            return Ok();
        }

        [HttpPost("login")]
        public ActionResult<User> Login([FromBody] UserAuthenticat model)
        {
            var user = _authenticationService.Login(model.Username, model.Password);

            if (user == null)
                return BadRequest("Username or password is incorrect");

            return Ok(user);
        }
    }
}