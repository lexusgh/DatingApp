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

        public AuthController(IAuthRepository repo)
        {
                    _repo = repo;
            
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

    }


}