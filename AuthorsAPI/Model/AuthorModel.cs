using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthorsAPI.Model
{
    public class AuthorModel : IdentityUser
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
