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
using DatingApp.API.Models;

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
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUser =  int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            userParams.UserId= currentUser;
            var curruser = await _repo.GetUser(currentUser);
            if(string.IsNullOrEmpty(userParams.Gender))
            {
               userParams.Gender = curruser.Gender == "male" ?"female" :"male" ;
            }
            var users = await _repo.GetUsers(userParams);
            
            var userdto = _mapper.Map<IEnumerable<UserDto>>(users);
            Response.AddPagination(users.CurrentPage,users.PageSize,
            users.TotalCount,users.TotalPages);

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

        [HttpPost("{id}/LikeUser/{recipientId}",Name="LikeUser")]
        public async Task<IActionResult> LikeUser(int id,int recipientId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var like= await _repo.GetLike(id,recipientId);
            if(like != null)
              return BadRequest("You already liked this user");

            if(await _repo.GetUser(recipientId) == null)
              return NotFound();

              like = new Models.Like{
                  LikerId = id,
                  LikeeId = recipientId
              };

              _repo.Add<Like>(like);
           if(await _repo.SaveAll())   
             return Ok();
             
          return BadRequest("Failed to like User");
        }
    }
}