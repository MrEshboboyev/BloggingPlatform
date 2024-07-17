using System.ComponentModel.DataAnnotations;

namespace BloggingPlatform.ViewModels
{
    public class EditUserViewModel
    {
        // avoid NullReferenceException
        public EditUserViewModel()
        {
            Claims = new List<String>();
            Roles = new List<String>();
        }

        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public List<String> Claims { get; set; }
        public IList<String> Roles { get; set; }
    }
}
