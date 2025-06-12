using JoyFix.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace JoyFix.Services
{
    public class TechnicianService
    {
        private readonly DynamicDbContextFactory _factory;
        public TechnicianService(DynamicDbContextFactory factory)
        {
            _factory = factory;
        }
        public List<Technician> GetAllTechnicians()
        {
            var _context = _factory.CreateDbContext();
            return _context.Technicians
                .AsNoTracking()
                .Include(t => t.Specializations)
                    .ThenInclude(ts => ts.Specialization)
                .Include(t => t.Repairs)
                .OrderBy(t => t.Name)
                .ToList();
        }

        public Technician? GetTechnicianById(int id)
        {
            var _context = _factory.CreateDbContext();
            return _context.Technicians
                .Include(t => t.Specializations)
                    .ThenInclude(ts => ts.Specialization)
                .FirstOrDefault(t => t.Id == id);
        }

        public List<Technician> GetTechniciansBySpecialization(string specialization)
        {
            var _context = _factory.CreateDbContext();
            return _context.Technicians
                .Where(t => t.Specializations.Any(s =>
                    s.Specialization.Name.Contains(specialization)))
                .AsNoTracking()
                .ToList();
        }

        public void AddTechnician(Technician technician)
        {
            var _context = _factory.CreateDbContext();
            if (_context.Technicians.Any(t => t.Email == technician.Email))
                throw new InvalidOperationException("Email is already taken by another technician.");

            if (!Regex.IsMatch(technician.PhoneNumber, @"^\d+$"))
                throw new InvalidOperationException("Phone number invalid");

            _context.Technicians.Add(technician);
            _context.SaveChanges();
        }

        public void UpdateTechnician(Technician technician)
        {
            var _context = _factory.CreateDbContext();
            var existing = _context.Technicians
                .Include(t => t.Specializations)
                .FirstOrDefault(t => t.Id == technician.Id);

            if (existing == null)
                throw new KeyNotFoundException("Technician not found.");

            if (_context.Technicians.Any(t => t.Email == technician.Email && t.Id != technician.Id))
                throw new InvalidOperationException("Email is already taken by another technician.");

            if (!Regex.IsMatch(technician.PhoneNumber, @"^\d+$"))
                throw new InvalidOperationException("Phone number invalid");

            existing.Name = technician.Name;
            existing.Email = technician.Email;
            existing.PhoneNumber = technician.PhoneNumber;

            _context.SaveChanges();
        }

        public void DeleteTechnician(int id)
        {
            var _context = _factory.CreateDbContext();
            var technician = _context.Technicians.Find(id);
            if (technician != null)
            {
                if (_context.Repairs.Any(r => r.TechnicianId == id))
                    throw new InvalidOperationException("Cannot delete technician with assigned repairs.");

                _context.Technicians.Remove(technician);
                _context.SaveChanges();
            }
        }

        public void AddSpecialization(int technicianId, int specializationId)
        {
            var _context = _factory.CreateDbContext();
            if (_context.TechnicianSpecializations
                .Any(ts => ts.TechnicianId == technicianId && ts.SpecializationId == specializationId))
                throw new InvalidOperationException("Technician already has this specialization.");

            var technicianSpecialization = new TechnicianSpecialization
            {
                TechnicianId = technicianId,
                SpecializationId = specializationId,
                AcquiredDate = DateTime.UtcNow
            };

            _context.TechnicianSpecializations.Add(technicianSpecialization);
            _context.SaveChanges();
        }

        public List<Specialization> GetAvailableSpecializations()
        {
            var _context = _factory.CreateDbContext();
            return _context.Specializations
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToList();
        }
    }
}