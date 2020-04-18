using System.ComponentModel;
using System.Text;
using System;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
                    _repo = repo;
                    _config = config;
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
               
            //validate request

            register.Username=register.Username.ToLower();

            if(await _repo.UserExists(register.Username))
                return BadRequest("User already exists!");

            var userToCreate =new User 
            {
                Username = register.Username
                
            };

            var createdUser = await _repo.Register(userToCreate,register.Password); 

            return StatusCode(201);
        }


         [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            
            //Check if user exists
            var user = await _repo.Login(login.username.ToLower(),login.password);

            if(user == null)
                //return BadRequest("Invalid Login!");
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username)
            };

            //Get Secret Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials  = creds

            };

            var tokenHandler  = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDesc);
            return Ok(new{
                token = tokenHandler.WriteToken(token)
            });

        }

    }


}