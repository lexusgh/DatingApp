using System.ComponentModel.DataAnnotations;
namespace DatingApp.API.Dtos
{
    public class LoginDto
    {
       
        public string username { get; set; }
       
        public string password { get; set; }
    }
}