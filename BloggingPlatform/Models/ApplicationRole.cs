using Microsoft.AspNetCore.Identity;

namespace BloggingPlatform.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}
