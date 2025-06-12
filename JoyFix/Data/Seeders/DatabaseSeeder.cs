using JoyFix.Data;
using JoyFix.Data.Seeders;

namespace JoyFix.Seeders
{
    public static class DatabaseSeeder
    {
        public static void Seed(ContextDB context)
        {
            CustomerSeeder.Seed(context);
            DeviceSeeder.Seed(context);
            TechnicianSeeder.Seed(context);
            RepairRequestSeeder.Seed(context);
            RepairSeeder.Seed(context);
        }
    }
}