using System;
using System.IO;

namespace SeparaArchivo
{
    class Program
    {
        static void Main(string[] args)
        {
            var sr = new StreamReader(@"C:\Users\rvenegas\Downloads\rv001.txt");
            var sw1 = new StreamWriter(@"C:\Users\rvenegas\Downloads\clientes1.txt");
            var sw2 = new StreamWriter(@"C:\Users\rvenegas\Downloads\clientes2.txt");
            var sw3 = new StreamWriter(@"C:\Users\rvenegas\Downloads\clientes3.txt");
            var sw4 = new StreamWriter(@"C:\Users\rvenegas\Downloads\clientes4.txt");
            var sw5 = new StreamWriter(@"C:\Users\rvenegas\Downloads\clientes5.txt");
            var sw6 = new StreamWriter(@"C:\Users\rvenegas\Downloads\clientes6.txt");
            var sw7 = new StreamWriter(@"C:\Users\rvenegas\Downloads\clientes7.txt");
            var sw8 = new StreamWriter(@"C:\Users\rvenegas\Downloads\clientes8.txt");
            var sw9 = new StreamWriter(@"C:\Users\rvenegas\Downloads\clientes9.txt");

            var cant = 0;

            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine();
                if (linea.EndsWith("_1_1"))
                    sw1.WriteLine(linea);
                if (linea.EndsWith("_1_2"))
                    sw2.WriteLine(linea);
                if (linea.EndsWith("_1_3"))
                    sw3.WriteLine(linea);
                if (linea.EndsWith("_1_4"))
                    sw4.WriteLine(linea);
                if (linea.EndsWith("_1_5"))
                    sw5.WriteLine(linea);
                if (linea.EndsWith("_1_6"))
                    sw6.WriteLine(linea);
                if (linea.EndsWith("_1_7"))
                    sw7.WriteLine(linea);
                if (linea.EndsWith("_1_8"))
                    sw8.WriteLine(linea);
                if (linea.EndsWith("_1_9"))
                    sw9.WriteLine(linea);

                cant++;
                if (cant > 1000)
                {
                    Console.WriteLine(cant);
                    cant = 0;
                }
            }

            sr.Close();
            sw1.Close();
            sw2.Close();
            sw3.Close();
            sw4.Close();
            sw5.Close();
            sw6.Close();
            sw7.Close();
            sw8.Close();
            sw9.Close();
        }
    }
}
