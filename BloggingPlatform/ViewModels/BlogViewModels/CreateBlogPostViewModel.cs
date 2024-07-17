using System.ComponentModel.DataAnnotations;

namespace BloggingPlatform.ViewModels.BlogViewModels
{
    public class CreateBlogPostViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The title must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [MinLength(50, ErrorMessage = "The content must be at least {1} characters long.")]
        public string Content { get; set; }
    }
}
