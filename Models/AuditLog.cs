using System.ComponentModel.DataAnnotations;

namespace Tp3.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public string TableName { get; set; } = string.Empty;

        [Required]
        public string Action { get; set; } = string.Empty; // "Added", "Modified", "Deleted"

        [Required]
        public string EntityKey { get; set; } = string.Empty;

        public string? Changes { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
