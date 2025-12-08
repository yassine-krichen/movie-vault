using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tp3.Models
{
    /// <summary>
    /// Represents the 'Movie' entity in the database.
    /// This class maps directly to the 'Movies' table.
    /// </summary>
    public class Movie
    {
        // Primary Key for the database table
        public int Id { get; set; }

        [Required] // Data Annotation: Ensures this field is not null in the DB
        public string Name { get; set; } = string.Empty;

        // Foreign key linking to the Genre table
        public int GenreId { get; set; }

        // Navigation property: Allows accessing the related Genre object directly
        // e.g., movie.Genre.GenreName
        public Genre? Genre { get; set; }

        // Stores the path to the image file (e.g., "/images/movies/abc.jpg")
        // We do NOT store the actual image binary data in the database for performance.
        public string? ImageFile { get; set; }

        // Date when movie was added
        public DateTime? DateAjoutMovie { get; set; }

        // Stock quantity
        public int Stock { get; set; }

        // Many-to-many relationship: A movie can be rented by many customers
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}