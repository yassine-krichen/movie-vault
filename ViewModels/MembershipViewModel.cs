using System.ComponentModel.DataAnnotations;

namespace Tp3.ViewModels
{
    public class MembershipViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Signup fee is required")]
        [Range(0, 1000000, ErrorMessage = "Signup fee must be between 0 and 1,000,000")]
        [Display(Name = "Signup Fee")]
        public decimal SignupFee { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 120, ErrorMessage = "Duration must be between 1 and 120 months")]
        [Display(Name = "Duration (Months)")]
        public int DurationInMonths { get; set; }

        [Required(ErrorMessage = "Discount rate is required")]
        [Range(0, 100, ErrorMessage = "Discount rate must be between 0 and 100")]
        [Display(Name = "Discount Rate (%)")]
        public byte DiscountRate { get; set; }
    }
}
