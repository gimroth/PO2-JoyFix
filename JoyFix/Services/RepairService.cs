using JoyFix.Data;
using Microsoft.EntityFrameworkCore;

namespace JoyFix.Services
{
    public class RepairService
    {
        private readonly DynamicDbContextFactory _factory;
        public RepairService(DynamicDbContextFactory factory)
        {
            _factory = factory;
        }
        public List<Repair> GetAllRepairs()
        {
            var _context = _factory.CreateDbContext();
            return _context.Repairs
                .AsNoTracking()
                .Include(r => r.Technician)
                .Include(r => r.RepairRequest)
                    .ThenInclude(rr => rr.Device)
                .OrderByDescending(r => r.RepairDate)
                .ToList();
        }

        public Repair? GetRepairByRequestId(int requestId)
        {
            var _context = _factory.CreateDbContext();
            return _context.Repairs
                .Include(r => r.Technician)
                .Include(r => r.RepairRequest)
                .FirstOrDefault(r => r.RepairRequestId == requestId);
        }

        public List<Repair> GetRepairsByTechnician(int technicianId)
        {
            var _context = _factory.CreateDbContext();
            return _context.Repairs
                .Where(r => r.TechnicianId == technicianId)
                .AsNoTracking()
                .ToList();
        }

        public List<Repair> GetRepairsByStatus(string status)
        {
            var _context = _factory.CreateDbContext();
            return _context.Repairs
                .Where(r => r.RepairRequest.Status == status)
                .AsNoTracking()
                .ToList();
        }

        public void AddRepair(Repair repair)
        {
            var _context = _factory.CreateDbContext();
            ValidateRepair(repair);
            if (_context.Repairs.Any(r => r.RepairRequestId == repair.RepairRequestId))
            {
                throw new InvalidOperationException("Repair for this request already exists.");
            }

            _context.Repairs.Add(repair);
            _context.SaveChanges();

            UpdateRepairRequestStatus(repair.RepairRequestId);
        }

        public void UpdateRepair(Repair repair)
        {
            var _context = _factory.CreateDbContext();
            var existing = _context.Repairs.Find(repair.RepairRequestId);
            if (existing == null)
                throw new KeyNotFoundException("Repair not found.");

            ValidateRepair(repair);

            existing.WorkDescription = repair.WorkDescription;
            existing.PartsUsed = repair.PartsUsed;
            existing.Cost = repair.Cost;
            existing.RepairDate = repair.RepairDate;
            existing.TechnicianId = repair.TechnicianId;

            _context.SaveChanges();
            UpdateRepairRequestStatus(repair.RepairRequestId);
        }

        public void DeleteRepair(int requestId)
        {
            var _context = _factory.CreateDbContext();
            var repair = _context.Repairs.Find(requestId);
            if (repair != null)
            {
                _context.Repairs.Remove(repair);
                _context.SaveChanges();
                UpdateRepairRequestStatus(requestId);
            }
        }

        private void ValidateRepair(Repair repair)
        {
            var _context = _factory.CreateDbContext();
            if (_context.Technicians.Find(repair.TechnicianId) == null)
                throw new KeyNotFoundException("Technician not found.");

            if (_context.RepairRequests.Find(repair.RepairRequestId) == null)
                throw new KeyNotFoundException("Repair request not found.");
        }

        private void UpdateRepairRequestStatus(int requestId)
        {
            var _context = _factory.CreateDbContext();
            var request = _context.RepairRequests.Find(requestId);
            if (request != null)
            {
                request.Status = _context.Repairs.Any(r => r.RepairRequestId == requestId)
                    ? "w trakcie"
                    : "oczekuje";
                _context.SaveChanges();
            }
        }
    }
}