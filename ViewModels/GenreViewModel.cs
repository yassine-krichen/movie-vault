using System.ComponentModel.DataAnnotations;

namespace Tp3.ViewModels
{
    public class GenreViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Genre name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Genre name must be between 2 and 50 characters")]
        [Display(Name = "Genre Name")]
        public string GenreName { get; set; } = string.Empty;
    }
}
