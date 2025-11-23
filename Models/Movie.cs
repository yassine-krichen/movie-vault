using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tp3.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Foreign key to Genre
        public int GenreId { get; set; }

        // Navigation property to Genre (many-to-one)
        public Genre? Genre { get; set; }

        // Image file path or URL
        public string? ImageFile { get; set; }

        // Date when movie was added
        public DateTime? DateAjoutMovie { get; set; }

        // Many-to-many: movies can be associated with many customers
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}