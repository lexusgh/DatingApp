using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         
         Task<bool> SaveAll();

         Task<PagedList<User>> GetUsers(UserParams userParams);

         Task<User> GetUser(int id);
         Task<Like> GetLike(int likerId,int likeeId);

        //  Task<Message> SendMessage(int senderId,int recipientId);
         Task<Message> GetMessage(int id);
         Task<PagedList<Message>> GetMessageForUser(MessageParams messageParams);
         Task<IEnumerable<Message>> GetMessageThread(int senderId,int recipientId);
         Task<Photo> GetPhoto(int id);
         Task<Photo> GetMainPhoto(int id);
    }
}