using DotnetJWTDapper.Models;
using DotnetJWTDapper.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetJWTDapper.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<ModelUserOut> users = _userRepository.GetUsers();
            if (users is object)
                return Ok(users);
            else
                return Ok(new List<ModelUser>());
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public IActionResult Login(ModelUser user)
        {
            try
            {
                ModelUserOut logged = _userRepository.Login(user.Email, user.Password);
                if (logged is object)
                    return Ok(logged);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public IActionResult Post([FromBody] ModelUser value)
        {
            try
            {
                string id = _userRepository.Add(value);
                return Created($"/api/users/{id}", value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            ModelUserOut user = _userRepository.GetUser(id);
            if (user is object)
                return Ok(user);
            else
                return NotFound();
        }

        [HttpPut("update")]
        public IActionResult Put([FromBody] ModelUser value)
        {
            bool updated = _userRepository.UpdateUser(value);
            if (updated)
                return Ok();
            else
                return NotFound();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(string id)
        {
            bool deleted = _userRepository.DeleteUser(id);
            if (deleted)
                return NoContent();
            else
                return NotFound();
        }
    }
}
