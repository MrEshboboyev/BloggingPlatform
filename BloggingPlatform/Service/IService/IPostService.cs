using BloggingPlatform.Models;

namespace BloggingPlatform.Service.IService
{
    public interface IPostService
    {
        Task<List<BlogPost>> GetPostsAsync(string userId);
        Task<BlogPost> GetPostByIdAsync(int postId);
        Task CreatePostAsync(BlogPost post);
        Task UpdatePostAsync(BlogPost post);
        Task DeletePostAsync(int postId);
    }
}
