using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLikeAsync(int sourceUserId, int likeUserId);

        Task<AppUser> GetUserWithLikesAsync(int userId);

        Task<PageList<LikeDto>> GetUserLikesAsync(LikesParams likesParams);  

        Task<bool> SaveAllAsync();
    }
}