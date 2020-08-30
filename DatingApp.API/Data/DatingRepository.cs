using System.Xml.XPath;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(m => m.Photos)
            .FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var user = await _context.Photos
            .FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var user = _context.Users.Include(m => m.Photos).OrderByDescending(x => x.LastActive).AsQueryable()
            ;
            user = user.Where(x => x.Id != userParams.UserId &&
             x.Gender == userParams.Gender);
            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var min = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var max = DateTime.Today.AddYears(-userParams.MinAge);

                user = user.Where(x => x.DateOfBirth >= min && x.DateOfBirth <= max);
            }

            if (userParams.Likers)
            {
                var uliker = await GetUserLikes(userParams.UserId, userParams.Likers);
                user = user.Where(i => uliker.Contains(i.Id));
            }

            if (userParams.Likees)
            {
                var ulikee = await GetUserLikes(userParams.UserId, userParams.Likers);
                user = user.Where(i => ulikee.Contains(i.Id));
            }

            if (!string.IsNullOrEmpty(userParams.OrderedBy))
            {
                switch (userParams.OrderedBy)
                {
                    case "created":
                        user = user.OrderByDescending(x => x.Created);
                        break;
                    default:
                        user = user.OrderByDescending(x => x.LastActive);
                        break;

                }
            }
            return await PagedList<User>.CreateAsync(user, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.LikerId == userId &&
            x.LikeeId == recipientId);
        }
        public async Task<Photo> GetMainPhoto(int id)
        {
            var user = await _context.Photos
                    .FirstOrDefaultAsync(x => x.UserId == id && x.IsMain);

            return user;
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.
            Include(x => x.Likers).Include(x => x.Likees).
            FirstOrDefaultAsync(b => b.Id == id);

            if (likers)
            {
                return user.Likers.Where(x => x.LikeeId == id).Select(o => o.LikerId);
            }

            return user.Likees.Where(x => x.LikerId == id).Select(o => o.LikeeId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<PagedList<Message>> GetMessageForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(a=> a.Sender ).ThenInclude(p=>p.Photos)
            .Include(a=>a.Recipient).ThenInclude(p=>p.Photos).AsQueryable();

            switch(messageParams.MessageContainer)
            {
                case "Inbox" :
                    messages = messages.Where(a => a.RecipientId == messageParams.UserId && !a.RecipientDeleted);
                    break;
                
                case "Outbox" :
                  messages = messages.Where(a => a.SenderId == messageParams.UserId && !a.SenderDeleted);
                    break;
                
                default :
                 messages = messages.Where(a => a.RecipientId == messageParams.UserId && !a.RecipientDeleted 
                 && !a.IsRead );
                 break;
            }

            messages  = messages.OrderByDescending(a=>a.MessageSent);
            return await PagedList<Message>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
             var messages = await _context.Messages.Include(a=> a.Sender ).ThenInclude(p=>p.Photos)
            .Include(a=>a.Recipient).ThenInclude(p=>p.Photos)
            .Where(a => a.RecipientId == userId && !a.RecipientDeleted && a.SenderId == recipientId
            || a.RecipientId == recipientId && !a.SenderDeleted && a.SenderId == userId)
            .OrderByDescending(a=>a.MessageSent).ToListAsync();

            return messages;
        }
    }
}