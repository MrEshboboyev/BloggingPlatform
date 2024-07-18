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

        public async Task DeletePostAsync(int postId)
        {
            var post = await _context.BlogPosts.FirstOrDefaultAsync(p => p.Id == postId);
            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task<BlogPost> GetPostByIdAsync(int postId)
        {
            return await _context.BlogPosts.FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<List<BlogPost>> GetPostsAsync(string userId)
        {
            return await _context.BlogPosts.
                Where(post => post.AuthorId == userId).
                ToListAsync();
        }

        public async Task UpdatePostAsync(BlogPost post)
        {
            try
            {
                _context.BlogPosts.Update(post);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
