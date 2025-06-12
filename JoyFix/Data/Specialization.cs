namespace JoyFix.Data
{
    public class Specialization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<TechnicianSpecialization> Technicians { get; set; } = new HashSet<TechnicianSpecialization>();
    }

    public class TechnicianSpecialization
    {
        public int TechnicianId { get; set; }
        public int SpecializationId { get; set; }
        public DateTime AcquiredDate { get; set; }

        public Technician Technician { get; set; }
        public Specialization Specialization { get; set; }
    }
}