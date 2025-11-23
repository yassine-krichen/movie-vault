using System.ComponentModel.DataAnnotations;

namespace Tp3.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Membership is required")]
        [Display(Name = "Membership")]
        public int MembershipId { get; set; }

        public string? MembershipInfo { get; set; }
    }
}
