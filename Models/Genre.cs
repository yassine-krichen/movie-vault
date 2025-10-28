using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tp3.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        public string GenreName { get; set; } = string.Empty;

        // Navigation: a genre can have many movies
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}