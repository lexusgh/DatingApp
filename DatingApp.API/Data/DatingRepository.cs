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
            var user= await _context.Users.Include(m=>m.Photos)
            .FirstOrDefaultAsync(x=>x.Id == id);

            return user;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var user= await _context.Photos
            .FirstOrDefaultAsync(x=>x.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var user=  _context.Users.Include(m=>m.Photos).OrderByDescending(x=>x.LastActive).AsQueryable()
            ;
            user =user.Where(x => x.Id != userParams.UserId && 
            x.Gender == userParams.Gender);
            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var min = DateTime.Today.AddYears(-userParams.MaxAge -1);
                var max = DateTime.Today.AddYears(-userParams.MinAge);

                user= user.Where(x => x.DateOfBirth >= min && x.DateOfBirth <= max);
            }

            if(!string.IsNullOrEmpty(userParams.OrderedBy))
            {
                switch (userParams.OrderedBy)
                {
                    case "created" :
                    user = user.OrderByDescending(x => x.Created);
                    break;
                    default :
                    user = user.OrderByDescending(x => x.LastActive);
                    break;

                }
            }
            return await PagedList<User>.CreateAsync(user,userParams.PageNumber,userParams.PageSize);
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
    }
}