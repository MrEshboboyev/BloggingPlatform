using BloggingPlatform.Models;
using BloggingPlatform.Service.IService;
using BloggingPlatform.ViewModels.BlogViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BloggingPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            IPostService postService,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _postService = postService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // convert List<BlogPostViewModel> and send data
            var listFromDb = await _postService.GetAllPostsAsync();

            var model = listFromDb.Select(bp => new BlogPostViewModel
            {
                Id = bp.Id,
                Content = bp.Content,   
                CreatedAt = bp.CreatedAt,   
                Title = bp.Title,
                UpdatedAt = bp.UpdatedAt,
                AuthorName = bp.Author.UserName,
                AuthorId = bp.AuthorId
            });

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ListPosts(string authorName)
        {
            var user = await _userManager.FindByNameAsync(authorName);

            if (user == null)
            {
                return NotFound();
            }

            // all posts type of BlogPost
            var listFromDb = await _postService.GetPostsAsync(user.Id);

            // this model is List<BlogPostViewModel>
            var model = listFromDb.Select(bp => new BlogPostViewModel
            {
                Id = bp.Id,
                Content = bp.Content,
                CreatedAt = bp.CreatedAt,
                Title = bp.Title,
                UpdatedAt = bp.UpdatedAt,
                AuthorId = bp.AuthorId,
                AuthorName = bp.Author.UserName
            });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PostDetails(int postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            var model = new BlogPostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                Title = post.Title,
                UpdatedAt = post.UpdatedAt,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.UserName
            };

            return View(model);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
