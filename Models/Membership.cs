using System.ComponentModel.DataAnnotations;

namespace Tp3.Models
{
    public class Membership
    {
        [Key]
        public int Id { get; set; }

        // Fee charged at signup (currency)
        [Range(0, 1000000)]
        public decimal SignupFee { get; set; }

        // Duration of the membership in months
        [Range(1, 120)]
        public int DurationInMonths { get; set; }

        // Discount rate as a percentage (0 - 100)
        [Range(0, 100)]
        public byte DiscountRate { get; set; }

        // Navigation: one membership can be assigned to many customers
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}