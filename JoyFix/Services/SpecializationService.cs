using JoyFix.Data;
using Microsoft.EntityFrameworkCore;

namespace JoyFix.Services
{
    public class SpecializationService
    {
        private readonly DynamicDbContextFactory _factory;
        public SpecializationService(DynamicDbContextFactory factory)
        {
            _factory = factory;
        }
        public List<Specialization> GetAllSpecializations()
        {
            var _context = _factory.CreateDbContext();
            return _context.Specializations
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToList();
        }

        public Specialization? GetSpecializationById(int id)
        {
            var _context = _factory.CreateDbContext();
            return _context.Specializations
                .Include(s => s.Technicians)
                .ThenInclude(ts => ts.Technician)
                .FirstOrDefault(s => s.Id == id);
        }

        public void AddSpecialization(Specialization specialization)
        {
            var _context = _factory.CreateDbContext();
            if (string.IsNullOrWhiteSpace(specialization.Name))
                throw new ArgumentException("Specialization name cannot be empty.");

            if (_context.Specializations.Any(s => s.Name == specialization.Name))
                throw new InvalidOperationException("Specialization with this name already exists.");

            _context.Specializations.Add(specialization);
            _context.SaveChanges();
        }

        public void UpdateSpecialization(Specialization specialization)
        {
            var _context = _factory.CreateDbContext();
            if (string.IsNullOrWhiteSpace(specialization.Name))
                throw new ArgumentException("Specialization name cannot be empty.");

            var existing = _context.Specializations
                .Include(s => s.Technicians)
                .FirstOrDefault(s => s.Id == specialization.Id);

            if (existing == null)
                throw new KeyNotFoundException("Specialization not found.");

            if (_context.Specializations.Any(s => s.Name == specialization.Name && s.Id != specialization.Id))
                throw new InvalidOperationException("Specialization with this name already exists.");

            existing.Name = specialization.Name;
            existing.Description = specialization.Description;

            _context.SaveChanges();
        }

        public void DeleteSpecialization(int id)
        {
            var _context = _factory.CreateDbContext();
            var specialization = _context.Specializations
                .Include(s => s.Technicians)
                .FirstOrDefault(s => s.Id == id);

            if (specialization == null)
                return;

            if (specialization.Technicians.Any())
                throw new InvalidOperationException("Cannot delete specialization assigned to technicians.");

            _context.Specializations.Remove(specialization);
            _context.SaveChanges();
        }
    }
}