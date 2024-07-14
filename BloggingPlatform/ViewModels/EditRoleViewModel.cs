using System.ComponentModel.DataAnnotations;

namespace BloggingPlatform.ViewModels
{
    public class EditRoleViewModel
    {
        public string RoleId { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        public string? Description { get; set; }
    }
}
