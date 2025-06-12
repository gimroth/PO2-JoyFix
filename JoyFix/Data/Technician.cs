using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JoyFix.Data
{
    public class Technician
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

        public ICollection<Repair> Repairs { get; set; } = new HashSet<Repair>();
        public ICollection<TechnicianSpecialization> Specializations { get; set; } = new HashSet<TechnicianSpecialization>();
    }
}