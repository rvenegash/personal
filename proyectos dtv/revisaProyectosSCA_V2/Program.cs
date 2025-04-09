using System;

namespace revisaProyectosSCA_V2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("revisaProyectosSCA_V2");

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "/run1":
                        var revisa = new revisaSCAfile(); //new revisaSCA();
                        revisa.procesar();
                        break;
                    case "/run2":
                        var reportes = new generaReportes();
                        reportes.GeneraReportes();

                        break;
                    case "/run3":
                        var graficos = new generaGV();
                        graficos.GeneraGV();

                        break;
                    default:
                        Console.WriteLine("Comando no reconocido!");
                        break;
                }
            }
            else
            {
                Console.WriteLine("No se enviaron parametros!");
            }
            Console.WriteLine("termino!");
        }
    }
}
