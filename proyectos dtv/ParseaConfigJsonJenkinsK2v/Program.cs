using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Text.Json;

namespace ParseaConfigJsonJenkinsK2v
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio");

            var baseGit = ConfigurationManager.AppSettings["BaseGIT"];
            var ambiente = ConfigurationManager.AppSettings["Ambiente"];
            var config = ConfigurationManager.AppSettings["ConfigFile"];
            var escribe = ConfigurationManager.AppSettings["escribe"];

            Console.WriteLine("Config: {0}", config);
            Console.WriteLine("Carpeta Base: {0}", baseGit);
            Console.WriteLine("Ambiente: {0}", ambiente);

            if (!File.Exists(config))
            {
                Console.WriteLine("Config no encontrado: {0}", config);
                return;
            }

            var sr = new StreamReader(config);
            var configJson = sr.ReadToEnd();
            sr.Close();

            var obj = JsonSerializer.Deserialize<Model.config>(configJson);
            foreach (var item in obj.ENVIRONMENTS)
            {
                if (item.NAME.Equals(ambiente))
                {
                    Console.WriteLine("Ambiente encontrado");

                    foreach (var ph in item.PLACEHOLDERS)
                    {
                        var arch = baseGit + ph.PATH;
                        Console.WriteLine("archivo: {0}", arch);
                        if (File.Exists(arch))
                        {
                            var swF = new StreamReader(arch);
                            var fstr = swF.ReadToEnd();
                            swF.Close();
                            foreach (var file in ph.PH)
                            {
                                var existe = fstr.Contains(file.NOMBRE);
                                Console.Write("   reemplazando : {0}", file.NOMBRE);
                                fstr = fstr.Replace(file.NOMBRE, file.VALOR);
                                if (existe && !fstr.Contains(file.NOMBRE))
                                {
                                    Console.Write("   reemplazado!");
                                }
                                Console.WriteLine();
                            }
                            if (escribe.Equals("true"))
                            {
                                var swW = new StreamWriter(arch, false);
                                swW.Write(fstr);
                                swW.Close();
                            }
                        }
                    }
                }
            }


            Console.WriteLine("fin");

            Console.ReadLine();
        }
    }
}
