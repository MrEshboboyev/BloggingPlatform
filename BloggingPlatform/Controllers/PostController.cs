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

        #region Delete Post

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            await _postService.DeletePostAsync(postId);
            return RedirectToAction("PostIndex");
        }

        #endregion

        #region Edit Post

        [HttpGet]
        public async Task<IActionResult> EditPost(int postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if(post == null)
            {
                ModelState.AddModelError(string.Empty, $"Post with Id : {postId} is not found in Database!");
                return View("Error");
            }

            var model = new EditBlogPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(EditBlogPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var post = await _postService.GetPostByIdAsync(model.Id);

                if (post == null)
                {
                    return NotFound();
                }

                post.Title = model.Title;
                post.Content = model.Content;
                post.UpdatedAt = DateTime.UtcNow;

                await _postService.UpdatePostAsync(post);

                return RedirectToAction("PostIndex");
            }

            return View(model);
        }

        #endregion
    }
}
