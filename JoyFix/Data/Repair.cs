using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoyFix.Data
{
    public class Repair
    {
        [Key]
        [ForeignKey("RepairRequest")] 
        public int RepairRequestId { get; set; } 

        public int TechnicianId { get; set; }

        [Required]
        public required string WorkDescription { get; set; }

        public string? PartsUsed { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal Cost { get; set; }

        public DateTime RepairDate { get; set; } = DateTime.UtcNow;
        public RepairRequest RepairRequest { get; set; } = null!;
        public Technician Technician { get; set; } = null!;
    }
}