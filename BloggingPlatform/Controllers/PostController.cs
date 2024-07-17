using BloggingPlatform.Data;
using BloggingPlatform.Models;
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

        #region Create Post

        [HttpGet]
        public IActionResult CreatePost()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreateBlogPostViewModel model)
        {
            if(ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                var post = new BlogPost
                {
                    Title = model.Title,
                    Content = model.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AuthorId = userId
                };

                await _postService.CreatePostAsync(post);
                return RedirectToAction("PostIndex");
            }

            return View(model);
        }

        #endregion
    }
}
