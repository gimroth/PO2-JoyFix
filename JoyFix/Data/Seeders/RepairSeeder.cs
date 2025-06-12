using Microsoft.EntityFrameworkCore;

namespace JoyFix.Data.Seeders
{
    public static class RepairSeeder
    {
        public static void Seed(ContextDB context)
        {
            if (context.Repairs.Any()) return;

            var requests = context.RepairRequests
                .Include(r => r.Device)
                .Where(r => r.Repair == null)
                .ToList();

            foreach (var request in requests)
            {
                var brand = request.DeviceType.Split(' ').First();

                var specializationMatch = brand switch
                {
                    "Nintendo" => "Naprawa Nintendo",
                    "PlayStation" => "Naprawa PlayStation",
                    "Xbox" => "Naprawa Xbox",
                    _ => null
                };

                if (specializationMatch == null) continue;

                var technician = context.Technicians
                    .Include(t => t.Specializations)
                    .ThenInclude(ts => ts.Specialization)
                    .FirstOrDefault(t => t.Specializations.Any(s => s.Specialization.Name == specializationMatch));

                if (technician == null) continue;

                var repair = new Repair
                {
                    RepairRequestId = request.Id,
                    TechnicianId = technician.Id,
                    WorkDescription = $"Naprawa: {request.IssueDescription}",
                    PartsUsed = $"Części do {request.Device.Model}",
                    Cost = new Random().Next(150, 300),
                    RepairDate = DateTime.UtcNow
                };

                context.Repairs.Add(repair);
                request.Status = "zakończone";
            }

            context.SaveChanges();
        }
    }
}
