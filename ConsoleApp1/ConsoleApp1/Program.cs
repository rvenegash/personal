using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();

            Console.WriteLine("Hello World!");
        }

        private static async Task MainAsync()
        {

            var serv = new ServiceReference1.ElementWSClient();

            var arch = @"C:\Trabajo\seriales_esales.txt";
            var sr = new System.IO.StreamReader(arch);
            var line = "";
            while ((line = sr.ReadLine()) != null)
            {
                var res = serv.getSerializedBySerialCodeAsync(line);
                var resp = await res;

            }

        }
        
    }
}
