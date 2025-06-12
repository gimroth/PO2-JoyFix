using System.ComponentModel.DataAnnotations;

namespace JoyFix.Data
{
    public class Device
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SerialNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string DeviceType { get; set; }

        [MaxLength(50)]
        public string Model { get; set; }

        public Customer Customer { get; set; }
        public ICollection<RepairRequest> RepairRequests { get; set; } = new HashSet<RepairRequest>();
    }
}