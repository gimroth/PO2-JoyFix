namespace JoyFix.Data.Seeders
{
    public static class TechnicianSeeder
    {
        public static void Seed(ContextDB context)
        {
            if (context.Technicians.Any()) return;

            var nintendoSpecialist = new Technician
            {
                Name = "Tomasz Wiśniewski",
                Email = "tomasz.wisniewski@joyfix.pl",
                PhoneNumber = "111222333",
                Specializations = new List<TechnicianSpecialization>
                {
                    new() { Specialization = GetOrCreateSpecialization(context, "Naprawa Nintendo", "Specjalista od Switch/3DS/Wii") },
                    new() { Specialization = GetOrCreateSpecialization(context, "Wymiana ekranów", "Naprawa wyświetlaczy") }
                }
            };

            var playstationSpecialist = new Technician
            {
                Name = "Karolina Nowak",
                Email = "karolina.nowak@joyfix.pl",
                PhoneNumber = "444555666",
                Specializations = new List<TechnicianSpecialization>
                {
                    new() { Specialization = GetOrCreateSpecialization(context, "Naprawa PlayStation", "PS5/PS4/PS Vita") },
                    new() { Specialization = GetOrCreateSpecialization(context, "Naprawa układów chłodzenia", "Wymiana past termicznych") }
                }
            };

            var xboxSpecialist = new Technician
            {
                Name = "Rafał Zieliński",
                Email = "rafal.zielinski@joyfix.pl",
                PhoneNumber = "777888999",
                Specializations = new List<TechnicianSpecialization>
                {
                    new() { Specialization = GetOrCreateSpecialization(context, "Naprawa Xbox", "Series X/S, One") },
                    new() { Specialization = GetOrCreateSpecialization(context, "Naprawa zasilaczy", "Diagnostyka zasilania") }
                }
            };

            context.Technicians.AddRange(nintendoSpecialist, playstationSpecialist, xboxSpecialist);
            context.SaveChanges();
        }

        private static Specialization GetOrCreateSpecialization(ContextDB context, string name, string description)
        {
            var spec = context.Specializations.FirstOrDefault(s => s.Name == name);
            if (spec == null)
            {
                spec = new Specialization { Name = name, Description = description };
                context.Specializations.Add(spec);
                context.SaveChanges();
            }

            return spec;
        }
    }
}
