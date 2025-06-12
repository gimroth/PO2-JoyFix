namespace JoyFix.Data.Seeders
{
    public static class DeviceSeeder
    {
        public static void Seed(ContextDB context)
        {
            if (context.Devices.Any()) return;

            var devices = new List<Device>
            {
                new Device
                {
                    CustomerId = 1,
                    DeviceType = "Nintendo Switch OLED",
                    Model = "OLED",
                    SerialNumber = $"NSO-{Guid.NewGuid().ToString()[..8]}"
                },
                new Device
                {
                    CustomerId = 1,
                    DeviceType = "Nintendo 3DS XL",
                    Model = "XL",
                    SerialNumber = $"3DS-{Guid.NewGuid().ToString()[..8]}"
                },
                new Device
                {
                    CustomerId = 2,
                    DeviceType = "PlayStation PS5 Digital Edition",
                    Model = "PS5",
                    SerialNumber = $"PS5-{Guid.NewGuid().ToString()[..8]}"
                },
                new Device
                {
                    CustomerId = 2,
                    DeviceType = "PlayStation PS Vita",
                    Model = "Vita",
                    SerialNumber = $"PSV-{Guid.NewGuid().ToString()[..8]}"
                },
                new Device
                {
                    CustomerId = 3,
                    DeviceType = "Xbox Series X",
                    Model = "Series X",
                    SerialNumber = $"XSX-{Guid.NewGuid().ToString()[..8]}"
                },
                new Device
                {
                    CustomerId = 3,
                    DeviceType = "Xbox One X",
                    Model = "One X",
                    SerialNumber = $"XOX-{Guid.NewGuid().ToString()[..8]}"
                }
            };

            context.Devices.AddRange(devices);
            context.SaveChanges();
        }
    }
}
