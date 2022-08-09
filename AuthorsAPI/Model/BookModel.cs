using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorsAPI.Model
{
    public class BookModel
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int PageNumber { get; set; } 

        [Required]
        public DateTime? DateOfPublication { get; set; }

        [Required]
        [ForeignKey(nameof(IdentityUser.Id))]
        public string AuthorId { get; set; } = string.Empty;
    }
}
