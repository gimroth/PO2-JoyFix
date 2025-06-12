using JoyFix.Data;
using Microsoft.EntityFrameworkCore;

namespace JoyFix.Services
{
    public class RepairRequestService
    {
        private readonly DynamicDbContextFactory _factory;
        public RepairRequestService(DynamicDbContextFactory factory)
        {
            _factory = factory;
        }
        public List<RepairRequest> GetAllRequests()
        {
            var _context = _factory.CreateDbContext();
            return _context.RepairRequests
                .AsNoTracking()
                .Include(rr => rr.Customer)
                .Include(rr => rr.Device)
                .Include(rr => rr.Repair)
                .OrderByDescending(rr => rr.CreatedAt)
                .ToList();
        }

        public RepairRequest? GetRequestById(int id)
        {
            var _context = _factory.CreateDbContext();
            return _context.RepairRequests
                .Include(rr => rr.Customer)
                .Include(rr => rr.Device)
                .Include(rr => rr.Repair)
                    .ThenInclude(r => r.Technician)
                .FirstOrDefault(rr => rr.Id == id);
        }

        public List<RepairRequest> GetRequestsByCustomer(int customerId)
        {
            var _context = _factory.CreateDbContext();
            return _context.RepairRequests
                .Where(rr => rr.CustomerId == customerId)
                .AsNoTracking()
                .ToList();
        }

        public List<RepairRequest> GetRequestsByDevice(int deviceId)
        {
            var _context = _factory.CreateDbContext();
            return _context.RepairRequests
                .Where(rr => rr.DeviceId == deviceId)
                .AsNoTracking()
                .ToList();
        }

        public void AddRequest(RepairRequest request)
        {
            var _context = _factory.CreateDbContext();
            ValidateRequest(request);

            request.CreatedAt = DateTime.UtcNow;
            request.Status = "oczekuje";

            _context.RepairRequests.Add(request);
            _context.SaveChanges();
        }

        public void UpdateRequest(RepairRequest request)
        {
            var _context = _factory.CreateDbContext();
            var existing = _context.RepairRequests.Find(request.Id);
            if (existing == null)
                throw new KeyNotFoundException("Request not found.");

            ValidateRequest(request);

            existing.DeviceType = request.DeviceType;
            existing.IssueDescription = request.IssueDescription;
            existing.DeviceId = request.DeviceId;
            existing.CustomerId = request.CustomerId;

            _context.SaveChanges();
        }

        public void UpdateRequestStatus(int id, string status)
        {
            var _context = _factory.CreateDbContext();
            var request = _context.RepairRequests.Find(id);
            if (request != null)
            {
                request.Status = status;
                _context.SaveChanges();
            }
        }

        public void DeleteRequest(int id)
        {
            var _context = _factory.CreateDbContext();
            var request = _context.RepairRequests
                .Include(rr => rr.Repair)
                .FirstOrDefault(rr => rr.Id == id);

            if (request != null)
            {
                if (request.Repair != null)
                    throw new InvalidOperationException("Cannot delete request with existing repair.");

                _context.RepairRequests.Remove(request);
                _context.SaveChanges();
            }
        }

        private void ValidateRequest(RepairRequest request)
        {
            var _context = _factory.CreateDbContext();
            if (_context.Customers.Find(request.CustomerId) == null)
                throw new KeyNotFoundException("Customer not found.");

            if (_context.Devices.Find(request.DeviceId) == null)
                throw new KeyNotFoundException("Device not found.");
        }
    }
}