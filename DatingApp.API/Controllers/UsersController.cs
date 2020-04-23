using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using DatingApp.API.Helpers;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();

            var userdto = _mapper.Map<IEnumerable<UserDto>>(users);

            return Ok(userdto);

        }
        [HttpGet("GetUser/{id}",Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            // if(user == null)
            //    return BadRequest("Not Found");
            var userdto = _mapper.Map<UserDetailsDto>(user);

            return Ok(userdto);
        }

        [HttpPut("UpdateUser/{id}",Name="UpdateUser")]
        public async Task<IActionResult> UpdateUser(int id,UserUpdateDto upd)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var user = await _repo.GetUser(id);
            // if(user == null)
            //    return BadRequest("Not Found");
            
            _mapper.Map(upd,user);

            if(await _repo.SaveAll())
               return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }
    }
}