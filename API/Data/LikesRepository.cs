using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext context;

        public LikesRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<UserLike> GetUserLikeAsync(int sourceUserId, int likeUserId)
        {
            return await context.Likes.FindAsync(sourceUserId, likeUserId);
        }

        public async Task<PageList<LikeDto>> GetUserLikesAsync(LikesParams likesParams)
        {
            var users = context.Users.OrderBy(u => u.Username).AsQueryable();

            var likes = context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }

            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                Username=user.Username,
                KnownAs=user.KnownAs,
                Age=user.DateOfBirth.CalculateAge(),
                PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain).Url,
                City=user.City,
                Id=user.Id
            });

            return await PageList<LikeDto>.CreateAsync(likedUsers,likesParams.PageNumber,likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikesAsync(int userId)
        {
            return await context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync()>0;
        }
    }
}