using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;

namespace RevisaCambiosCarpetasOSB
{
    class Program
    {
        static MySqlConnection conn;
        static string informe;

        static void Main(string[] args)
        {
            var mysqldb = ConfigurationManager.AppSettings["MySQL160"];
            conn = new MySqlConnection(mysqldb);
            conn.Open();

            informe = ConfigurationManager.AppSettings["CarpetaInforme"];
            revisarCarpetasNuevas();

            revisarCarpetasConCambios();

            conn.Close();
        }

        /// <summary>
        /// revisar carpetas con cambios
        /// </summary>
        private static void revisarCarpetasConCambios()
        {
            StreamWriter arch = new StreamWriter(informe + "carpetas-cambios.txt");

            //leer todas las carpetas
            var carpetas = new List<CarpetaRevisar>();
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select * from carpetas_osb where revisar = 'S' ";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("Tipo\tCarpeta\tArchivo\tFecha");
                while (reader.Read())
                {
                    if (reader["subcarpetas"].ToString() == "")
                    {
                        carpetas.Add(new CarpetaRevisar() { Tipo = reader["tipo"].ToString(), Carpeta = reader["carpeta"].ToString(), Fecha = (DateTime)reader["fecha_actualizacion"] });
                    }
                    else
                    {
                        foreach (var item in reader["subcarpetas"].ToString().Split(','))
                        {
                            carpetas.Add(new CarpetaRevisar() { Tipo = reader["tipo"].ToString(), Carpeta = reader["carpeta"].ToString() + "\\" + item, Fecha = (DateTime)reader["fecha_actualizacion"] });
                        }
                    }
                }
                reader.Close();
                reader = null;
            }
            foreach (var item in carpetas)
            {
                item.Carpeta = (item.Tipo == "F" ? ConfigurationManager.AppSettings["OSBFront-BaseFolder"] + item.Carpeta : ConfigurationManager.AppSettings["OSBBack-BaseFolder"] + item.Carpeta);
                LeeCarpetaRecursiva(item, arch);
            }

            arch.Close();
        }

        static void LeeCarpetaRecursiva(CarpetaRevisar carpeta, StreamWriter log)
        {
            if (carpeta.Carpeta == ConfigurationManager.AppSettings["OSBFront-BaseFolder"] || carpeta.Carpeta == ConfigurationManager.AppSettings["OSBBack-BaseFolder"]) { return; }

            Console.WriteLine("Revisando carpeta: " + carpeta.Carpeta);
            var list = Directory.GetDirectories(carpeta.Carpeta);
            foreach (var item in list)
            {
                var listF = Directory.GetFiles(item, "*.*");
                foreach (var itemF in listF)
                {
                    if (itemF.EndsWith(".proxy") || itemF.EndsWith(".wsdl") || itemF.EndsWith(".xsd") || itemF.EndsWith("RoutingTable.xq") || itemF.EndsWith(".biz"))
                    {
                        if (File.GetCreationTime(itemF) > carpeta.Fecha || File.GetLastWriteTime(itemF) > carpeta.Fecha)
                        {
                            log.WriteLine("{0}\t{1}\t{2}\t{3}", carpeta.Tipo, carpeta.Carpeta, itemF, File.GetCreationTime(itemF).ToString());
                        }
                    }
                }

                Console.WriteLine("Revisando sub carpetas ");
                carpeta.Carpeta = item;
                LeeCarpetaRecursiva(carpeta, log);
            }
        }


        /// <summary>
        /// detectar nuevos folders
        /// </summary>
        private static void revisarCarpetasNuevas()
        {
            StreamWriter arch = new StreamWriter(informe + "carpetas-nuevas.txt");

            //leer todas las carpetas
            List<string> carpetas = new List<string>();
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select * from carpetas_osb";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("Tipo\tCarpeta");
                while (reader.Read())
                {
                    carpetas.Add(reader["tipo"].ToString() + "#" + reader["carpeta"].ToString());

                }
                reader.Close();
                reader = null;
            }

            string carpeta = ConfigurationManager.AppSettings["OSBFront-BaseFolder"];
            var list = Directory.GetDirectories(carpeta);
            foreach (var item in list)
            {
                var dir = Path.GetFileName(item);
                if (!carpetas.Contains("F#" + dir))
                {
                    arch.WriteLine("{0}\t{1}", "F", dir);
                }
            }
            carpeta = ConfigurationManager.AppSettings["OSBBack-BaseFolder"];
            list = Directory.GetDirectories(carpeta);
            foreach (var item in list)
            {
                var dir = Path.GetFileName(item);
                if (!carpetas.Contains("B#" + dir))
                {
                    arch.WriteLine("{0}\t{1}", "B", dir);
                }
            }

            arch.Close();
        }
    }

    class CarpetaRevisar
    {
        public string Tipo { get; set; }
        public string Carpeta { get; set; }
        public DateTime Fecha { get; set; }

    }
}
