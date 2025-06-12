namespace JoyFix.Data.Seeders
{
    public static class CustomerSeeder
    {
        public static void Seed(ContextDB context)
        {
            if (context.Customers.Any()) return;

            var customers = new List<Customer>
            {
                new Customer
                {
                    Name = "Adam Nowak",
                    Email = "adam.nowak@example.com",
                    PhoneNumber = "111222333",
                    Address = "ul. Kwiatowa 10, Warszawa",
                },
                new Customer
                {
                    Name = "Beata Kowalska",
                    Email = "beata.kowalska@example.com",
                    PhoneNumber = "444555666",
                    Address = "ul. Lipowa 5, Kraków", 
                },
                new Customer
                {
                    Name = "Michał Zieliński",
                    Email = "michal.zielinski@example.com",
                    PhoneNumber = "777888999",
                    Address = "ul. Dębowa 7, Gdańsk",
                }
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();
        }
    }
}
