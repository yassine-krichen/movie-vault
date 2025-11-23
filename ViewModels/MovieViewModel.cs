using System.ComponentModel.DataAnnotations;

namespace Tp3.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Movie name is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Movie name must be between 1 and 200 characters")]
        [Display(Name = "Movie Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Genre is required")]
        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        [Display(Name = "Image File")]
        [StringLength(500, ErrorMessage = "Image file path cannot exceed 500 characters")]
        public string? ImageFile { get; set; }

        [Display(Name = "Date Added")]
        [DataType(DataType.Date)]
        public DateTime? DateAjoutMovie { get; set; }

        public string? GenreName { get; set; }
    }
}
