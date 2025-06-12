using Microsoft.EntityFrameworkCore;

namespace JoyFix.Data.Seeders
{
    public static class RepairRequestSeeder
    {
        public static void Seed(ContextDB context)
        {
            if (context.RepairRequests.Any()) return;

            var issuesByBrand = new Dictionary<string, (string Title, string Desc, string Specialization)[]>
            {
                { "Nintendo", new[] {
                    ("Drift Joy-Con", "Analogowe drążki reagują samoistnie", "Naprawa Nintendo"),
                    ("Uszkodzony ekran", "Pęknięty wyświetlacz dotykowy", "Wymiana ekranów")
                }},
                { "PlayStation", new[] {
                    ("Przegrzewanie", "Konsola wyłącza się podczas gry", "Naprawa układów chłodzenia"),
                    ("Brak obrazu HDMI", "Uszkodzony port wideo", "Naprawa PlayStation")
                }},
                { "Xbox", new[] {
                    ("Problem z zasilaczem", "Konsola nie włącza się", "Naprawa zasilaczy"),
                    ("Awaria dysku", "Błędy przy odczycie gier", "Naprawa Xbox")
                }}
            };

            foreach (var (brand, issues) in issuesByBrand)
            {
                var customers = context.Customers
                    .Include(c => c.Devices)
                    .Where(c => c.Devices.Any(d => d.DeviceType.Contains(brand)))
                    .ToList();

                foreach (var customer in customers)
                {
                    var device = customer.Devices.FirstOrDefault(d => d.DeviceType.Contains(brand));
                    if (device == null) continue;

                    foreach (var (title, desc, spec) in issues)
                    {
                        context.RepairRequests.Add(new RepairRequest
                        {
                            CustomerId = customer.Id,
                            DeviceId = device.Id,
                            DeviceType = device.DeviceType,
                            IssueDescription = $"{brand} - {title}: {desc}",
                            Status = "oczekuje",
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
