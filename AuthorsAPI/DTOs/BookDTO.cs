using System.ComponentModel.DataAnnotations;

namespace AuthorsAPI.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int PageNumber { get; set; } 

        [Required]
        public DateTime DateOfPublication { get; set; }
    }

    public class CreateBookDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int PageNumber { get; set; } 

        [Required]
        //[DataType(DataType.Date)]
        public DateTime DateOfPublication { get; set; }
    }
}
