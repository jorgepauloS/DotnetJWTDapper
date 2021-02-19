using DotnetJWTDapper.Models;
using DotnetJWTDapper.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DotnetJWTDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult Token([FromBody] RequestTokenIn request)
        {
            ModelUser modelUser = _userRepository.GetUserByToken(request.Token);

            //Verify user and token expiration
            if (modelUser is object && modelUser.TokenExpiration >= DateTime.UtcNow)
                return Success(modelUser);

            return NotFound(new RequestTokenOut() { ErrorMessage = "Invalid Token" });
        }

        [AllowAnonymous]
        [HttpPost("refreshtoken")]
        public IActionResult RefreshToken([FromBody] RequestRefreshTokenIn request)
        {
            //Verify user and refresh token expiration
            ModelUser modelUser = _userRepository.GetUserByRefreshToken(request.RefreshToken);

            if (modelUser is object && modelUser.RefreshTokenExpiration >= DateTime.UtcNow)
            {
                //Update user token and return the new token
                modelUser = _userRepository.RefreshToken(modelUser);
                return Success(modelUser);
            }

            return NotFound(new RequestTokenOut() { ErrorMessage = "Invalid Token" });
        }

        private IActionResult Success(ModelUser pUser)
        {
            //Define claims
            var claims = new[]
            {
                new Claim("token", pUser.Token),
                new Claim(ClaimTypes.Name, pUser.Username),
                new Claim(ClaimTypes.NameIdentifier, pUser.Id)
            };

            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));

            //Define SigninCredential
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Generate JWT Token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: pUser.TokenExpiration,
                issuer: "softwareengineer",
                audience: pUser.Name,
                signingCredentials: creds
                );

            return Ok(new RequestTokenOut
            {
                Bearer = new JwtSecurityTokenHandler().WriteToken(token),
                Token = pUser.Token,
                TokenExpiration = pUser.TokenExpiration,
                RefreshToken = pUser.RefreshToken,
                RefreshTokenExpiration = pUser.RefreshTokenExpiration
            });
        }
    }
}
