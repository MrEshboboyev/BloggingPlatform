using BloggingPlatform.Data;
using BloggingPlatform.Models;
using BloggingPlatform.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatform.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreatePostAsync(BlogPost post)
        {
            await _context.BlogPosts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public Task DeletePostAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<BlogPost> GetPostByIdAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BlogPost>> GetPostsAsync(string userId)
        {
            return await _context.BlogPosts.
                Where(post => post.AuthorId == userId).
                ToListAsync();
        }

        public Task UpdatePostAsync(BlogPost post)
        {
            throw new NotImplementedException();
        }
    }
}
