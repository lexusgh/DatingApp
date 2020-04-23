using System;
using System.Reflection.Metadata;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userid}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryconfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, 
        IOptions<CloudinarySettings> cloudinaryconfig)
        {
            _cloudinaryconfig = cloudinaryconfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account(
                _cloudinaryconfig.Value.CloudName,
                _cloudinaryconfig.Value.ApiKey,
                _cloudinaryconfig.Value.ApiSecret
            );
                
            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}" ,Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        
        public async Task<IActionResult> AddUserPhoto(int userid,[FromForm]UserPhotoDto uphoto)
        {
            if(userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var user = await _repo.GetUser(userid);

            var file = uphoto.File;

            var uploadResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation().
                    Width(500).Height(500).Crop("fill").Gravity("face")
                };

                uploadResult = _cloudinary.Upload(uploadParams);

            }

            uphoto.Url = uploadResult.Uri.ToString();
            uphoto.PublicId = uploadResult.PublicId;
            var photo = _mapper.Map<Photo>(uphoto);

            if(!user.Photos.Any(x=>x.IsMain))
                photo.IsMain = true;

            user.Photos.Add(photo);

           if(await _repo.SaveAll())
             {
                 var returnphoto = _mapper.Map<PhotoReturnDto>(photo);
                 return CreatedAtRoute("GetPhoto",new {userId= userid,id=photo.Id},returnphoto);  
             }
                

            return BadRequest("could not add the photo");
        }


        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMain(int userid,int id)
        {
            if(userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _repo.GetUser(userid);
            if(!user.Photos.Any(x=>x.Id == id))
                return Unauthorized();

               
            var photoFromRepo = await _repo.GetPhoto(id);
            if(photoFromRepo.IsMain)
                return BadRequest("this is already the main photo");
             var mainphoto = await _repo.GetMainPhoto(userid);
             mainphoto.IsMain = false;
             photoFromRepo.IsMain = true;

             if(await _repo.SaveAll())
                return NoContent();            
                //  var returnphoto = _mapper.Map<PhotoReturnDto>(photoFromRepo);
                //  return Ok(returnphoto );  
             
             
                

            return BadRequest("photo update failed");

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userid,int id)
        {
            if(userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _repo.GetUser(userid);
            if(!user.Photos.Any(x=>x.Id == id))
                return Unauthorized();
            
               var delphoto = await _repo.GetPhoto(id);

                 if(delphoto.IsMain)
                   return BadRequest("you cannot delete main photo");

                   if(delphoto.PublicId != null){
                       var deletionParams = new DeletionParams(delphoto.PublicId);
                        var result = _cloudinary.Destroy(deletionParams);
                            if(result.Result == "ok")
                            {
                            _repo.Delete(delphoto);
                                
                            }  
                    }
               
                   if(delphoto.PublicId != null)
                      _repo.Delete(delphoto);
                    
                    if(await _repo.SaveAll())
                        return Ok();    
                       
                //  var returnphoto = _mapper.Map<PhotoReturnDto>(photoFromRepo);
                //  return Ok(returnphoto );  
             
             

                

            return BadRequest("photo deletion failed");
        }

    }
    
}