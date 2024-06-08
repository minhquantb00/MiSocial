using Hangfire;

namespace BackgrounJobs.Proccess
{
    class Program
    {
        static void Main(string[] args)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("Server=localhost;Integrated Security=true;Initial Catalog=DatabaseAdmission;");

            using (var server = new BackgroundJobServer())
            {
                Console.WriteLine("Hangfire Server started. Press any key to exit...");
                Console.ReadKey();
            }

        }
    }
}