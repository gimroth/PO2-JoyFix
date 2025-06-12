using JoyFix.Data;
using Microsoft.EntityFrameworkCore;

namespace JoyFix.Services
{
    public class DeviceService
    {
        private readonly DynamicDbContextFactory _factory;
        public DeviceService(DynamicDbContextFactory factory)
        {
            _factory = factory;
        }

        public List<Device> GetAllDevices()
        {
            var _context = _factory.CreateDbContext();
            return _context.Devices
                .AsNoTracking()
                .Include(d => d.Customer)
                .Include(d => d.RepairRequests)
                .ToList();
        }

        public Device? GetDeviceById(int id)
        {
            var _context = _factory.CreateDbContext();
            return _context.Devices
                .Include(d => d.Customer)
                .FirstOrDefault(d => d.Id == id);
        }

        public List<Device> GetDevicesByCustomer(int customerId)
        {
            var _context = _factory.CreateDbContext();
            return _context.Devices
                .Where(d => d.CustomerId == customerId)
                .AsNoTracking()
                .ToList();
        }

        public void AddDevice(Device device)
        {
            var _context = _factory.CreateDbContext();
            if (_context.Devices.Any(d => d.SerialNumber == device.SerialNumber))
                throw new InvalidOperationException("Serial number must be unique.");

            if (_context.Customers.Find(device.CustomerId) == null)
                throw new KeyNotFoundException("Customer not found.");

            _context.Devices.Add(device);
            _context.SaveChanges();
        }

        public void UpdateDevice(Device device)
        {
            var _context = _factory.CreateDbContext();
            var existing = _context.Devices.Find(device.Id);
            if (existing == null)
                throw new KeyNotFoundException("Device not found.");

            if (_context.Devices.Any(d => d.SerialNumber == device.SerialNumber && d.Id != device.Id))
                throw new InvalidOperationException("Serial number is already used by another device.");

            existing.SerialNumber = device.SerialNumber;
            existing.DeviceType = device.DeviceType;
            existing.Model = device.Model;
            existing.CustomerId = device.CustomerId;

            _context.SaveChanges();
        }

        public void DeleteDevice(int id)
        {
            var _context = _factory.CreateDbContext();
            var device = _context.Devices.Find(id);
            if (device != null)
            {
                // Sprawdź czy urządzenie nie ma powiązanych napraw
                if (_context.RepairRequests.Any(r => r.DeviceId == id))
                    throw new InvalidOperationException("Cannot delete device with existing repair requests.");

                _context.Devices.Remove(device);
                _context.SaveChanges();
            }
        }

        public List<string> GetAvailableDeviceTypes()
        {
            var _context = _factory.CreateDbContext();
            return _context.Devices
                .Select(d => d.DeviceType)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }
    }
}