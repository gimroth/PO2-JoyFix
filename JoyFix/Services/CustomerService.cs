using JoyFix.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace JoyFix.Services
{
    public class CustomerService
    {

        private readonly DynamicDbContextFactory _factory;

        public CustomerService(DynamicDbContextFactory factory)
        {
            _factory = factory;
        }

        public List<Customer> GetAllCustomers()
        {
            var _context = _factory.CreateDbContext();

            return _context.Customers
                .AsNoTracking()
                .Include(c => c.Devices)
                .Include(c => c.RepairRequests)
                .ToList();
        }

        public Customer? GetCustomerById(int id)
        {
            var _context = _factory.CreateDbContext();

            return _context.Customers.Find(id);
        }

        public void AddCustomer(Customer customer)
        {
            var _context = _factory.CreateDbContext();
            if (_context.Customers.Any(c => c.Email == customer.Email))
                throw new InvalidOperationException("Email is already taken by another customer.");

            if (!Regex.IsMatch(customer.PhoneNumber, @"^\d+$"))
                throw new InvalidOperationException("Phone number invalid");

            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void UpdateCustomer(Customer customer)
        {
            var _context = _factory.CreateDbContext();
            var existing = _context.Customers.Find(customer.Id);
            if (existing == null)
                throw new KeyNotFoundException("Customer not found.");

            if (_context.Customers.Any(c => c.Email == customer.Email && c.Id != customer.Id))
                throw new InvalidOperationException("Email is already taken by another customer.");

            if (!Regex.IsMatch(customer.PhoneNumber, @"^\d+$"))
                throw new InvalidOperationException("Phone number invalid");

            existing.Name = customer.Name;
            existing.Email = customer.Email;
            existing.PhoneNumber = customer.PhoneNumber;
            existing.Address = customer.Address;

            _context.SaveChanges();
        }

        public void DeleteCustomer(int id)
        {
            var _context = _factory.CreateDbContext();
            var customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
        }
    }
}
