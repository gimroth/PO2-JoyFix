using System.ComponentModel.DataAnnotations;

namespace JoyFix.Data
{
    public class RepairRequest
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public int DeviceId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string DeviceType { get; set; }

        [Required]
        public required string IssueDescription { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "oczekuje";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Repair? Repair { get; set; }

        public Customer Customer { get; set; } = null!;
        public Device Device { get; set; } = null!;
    }
}
