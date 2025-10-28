using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tp3.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Foreign key to Membership
        public int MembershipId { get; set; }

        // Navigation property
        public Membership? Membership { get; set; }

        // Many-to-many: customers can have many movies (e.g., rented/favorite)
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}