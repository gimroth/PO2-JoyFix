using System.ComponentModel.DataAnnotations;

namespace JoyFix.Data
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        public ICollection<Device> Devices { get; set; } = new HashSet<Device>();
        public ICollection<RepairRequest> RepairRequests { get; set; } = new HashSet<RepairRequest>();
    }
}