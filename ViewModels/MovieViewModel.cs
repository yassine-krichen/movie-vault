using System.ComponentModel.DataAnnotations;

namespace Tp3.ViewModels
{
    /// <summary>
    /// A Data Transfer Object (DTO) specifically for the View.
    /// It contains only the data needed to display or edit a movie form.
    /// It separates the internal database model (Movie) from the user interface.
    /// </summary>
    public class MovieViewModel
    {
        public int Id { get; set; }

        // Validation attributes control what the user sees in the form
        [Required(ErrorMessage = "Movie name is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Movie name must be between 1 and 200 characters")]
        [Display(Name = "Movie Name")] // Label shown in the HTML form
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Genre is required")]
        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        // This property holds the path to display the image (e.g., <img src="@Model.ImageFile">)
        // Note: The actual file upload comes in a separate IFormFile parameter in the Controller.
        [Display(Name = "Image File")]
        [StringLength(500, ErrorMessage = "Image file path cannot exceed 500 characters")]
        public string? ImageFile { get; set; }

        [Display(Name = "Date Added")]
        [DataType(DataType.Date)] // Tells the browser to render a date picker
        public DateTime? DateAjoutMovie { get; set; }

        // Extra property for display purposes only (not saved to DB directly from here)
        public string? GenreName { get; set; }
    }
}
