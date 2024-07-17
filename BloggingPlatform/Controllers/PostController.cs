using BloggingPlatform.Data;
using BloggingPlatform.Service.IService;
using BloggingPlatform.ViewModels.BlogViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BloggingPlatform.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly UserManager<IdentityUser> _userManager;

        public PostController(IPostService postService,
            UserManager<IdentityUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }

        public async Task<IActionResult> PostIndex()
        {
            var userId = _userManager.GetUserId(User);
            var blogPostList = await _postService.GetPostsAsync(userId);

            var model = blogPostList.Select(post => new BlogPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            });

            return View(model);
        }
    }
}
