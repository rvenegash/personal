using MySql.Data.MySqlClient;
using revisaProyectosSCA_V2.Clases;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

namespace revisaProyectosSCA_V2
{
    public class generaGV
    {
        public void GeneraGV()
        {
            var mysqldb = ConfigurationManager.AppSettings["MySQL160"];
            MySqlConnection conn = new MySqlConnection(mysqldb);
            conn.Open();

            //leer las APPs
            var lApp = new List<Aplicacion>();

            //ruta a DOT
            var dotEXE = ConfigurationManager.AppSettings["DotEXE"];
            var listaDeprecados = new List<string>() { "53125", "50930", "50932", "50929", "50071", "50931", "53151", "50075", "50076", "50077", "50078", "50079", "50080", "50081", "53175" };

            var sql = "";
            sql = "select id, name from asset a where a.type = 158";
            // sql = "select id, name from asset a where a.id = 50746";
            /*
             * 
    50200	Topic
    50102	Functionality
    50101	Module
    50100	ETL
    50009	BusinessEntity
    50008	DataBase
    50007	Message Queue
    50006	Country
    50005	DataCenter
    50004	Operation
    193	BusinessProcess
    187	Interface
    169	Endpoint
    158	Application
    154	Service
    145	Component
    144	Framework

             * */
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lApp.Add(new Aplicacion() { Id = reader["id"].ToString(), Name = reader["name"].ToString() });
                    }
                }
            }

            Console.WriteLine("apps: " + lApp.Count);

            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var nombre_arch_cmd = ConfigurationManager.AppSettings["PathGV"] + "genera_todo." + (isWindows ? "cmd" : "sh"); 
            var arch_cmd = new StreamWriter(nombre_arch_cmd, false);

            //Console.WriteLine("..{0}Data{0}uploads{0}{{filename}}", Path.DirectorySeparatorChar);


            var st_ServDeprecado = ConfigurationManager.AppSettings["St_ServicioDeprecado"];
            var st_ServActivo = ConfigurationManager.AppSettings["St_ServicioActivo"];
            var st_Operacion = ConfigurationManager.AppSettings["St_Operacion"];
            var st_Servicio = ConfigurationManager.AppSettings["St_Servicio"];

            foreach (var app in lApp)
            {
                //limpiar variables
                var lInv = new List<string>();
                var lServ = new List<Servicio>();

                //buscar relaciones
                sql = "select ad.id, ad.name, ad.version as oper_version, ad2.id service_id, ad2.name service_name , ad2.version as service_version " +
                    "from asset a " +
                    "inner join asset_relation ar on ar.ASSET_ID = a.id " +
                    "inner join asset ad on ad.id = ar.RELATED_ASSET_ID " +
                    "inner join asset a2 on  a2.id = ad.id " +
                    "inner join asset_relation ar2 on ar2.ASSET_ID = a2.id " +
                    "inner join asset ad2 on ad2.id = ar2.RELATED_ASSET_ID " +
                    "where a.id = " + app.Id + " and ar.RELATION_TYPE = 50000 and ar.RELATED_PS = 'P' and " +
                    " ar2.RELATION_TYPE = 108 and ar2.RELATED_PS = 'P' " +
                    "order by 3, 2";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var serv = new Servicio() { Id = reader["service_id"].ToString(), Name = reader["service_name"].ToString(), Version = reader["service_version"].ToString() };
                            var oper = new Operacion() { Id = reader["id"].ToString(), Name = reader["name"].ToString(), Version = reader["oper_version"].ToString() };

                            guardaOper(ref lServ, serv, oper);

                            lInv.Add(oper.Id);
                        }
                    }
                }

                //armar listas
                if (lInv.Count > 0)
                {
                    //crear archivo
                    var nombre_arch = ConfigurationManager.AppSettings["PathGV"] + string.Format("{0}-{1}.gv", app.Id, RemoveAcentuation(app.Name.Trim()));
                    var nombre_arch_corto = ConfigurationManager.AppSettings["PathGV"] + string.Format("{0}.png", app.Id);
                    var arch = new StreamWriter(nombre_arch, false);
                    Console.WriteLine("Armando archivo: {0}", nombre_arch);

                    //encabezado del archivo
                    arch.WriteLine("//Level 0");
                    arch.WriteLine("digraph Model_{0}", app.Id);
                    arch.WriteLine("{");
                    arch.WriteLine("	splines= polyline; fontname=Verdana;");
                    //splines: splines, none, ortho, curved, polyline
                    arch.WriteLine("	subgraph level0 {");
                    arch.WriteLine("		{0} [label=\"{1}\" {2}];", app.Id, app.Name, st_Servicio);
                    arch.WriteLine("	}");

                    //vaciar listas a archivo
                    arch.WriteLine("	//services");
                    foreach (var item in lServ)
                    {
                        arch.WriteLine("	subgraph cluster_{0}", item.Id);
                        arch.WriteLine("	{");
                        if (listaDeprecados.Contains(item.Id))
                        {
                            arch.WriteLine("	label =\"{0} ({1}) (deprecado)\";", item.Name, item.Version);
                            arch.WriteLine("	{0}", st_ServDeprecado);
                        }
                        else
                        {
                            arch.WriteLine("	label =\"{0} ({1})\";", item.Name, item.Version);
                            arch.WriteLine("	{0}", st_ServActivo);
                        }

                        foreach (var item2 in item.Operaciones)
                        {
                            arch.WriteLine("		{0} [label=\"{1} ({2})\" {3}];", item2.Id, item2.Name, item2.Version, st_Operacion);
                        }

                        arch.WriteLine("	}");
                    }

                    //escribir relaciones
                    arch.WriteLine("	//relations");
                    foreach (var item in lInv)
                    {
                        arch.WriteLine("	{0}->{1}", app.Id, item);
                    }

                    arch.WriteLine("}");
                    //cerrar archivo
                    arch.Close();

                    //http://www.graphviz.org/Documentation.php

                    //C:\RESPALDOS\Herramientas\graphviz\bin>dot -Tpng -Kfdp -O "C:\Trabajo\revision\50025-ABDNET - AR.gv"
                    //formatos: PNG, SVG (vectorial)
                    //tipo de grafico: dot, naneo?, fdp

                    arch_cmd.WriteLine("{0} -Tpng -Kfdp -o{1} \"{2}\"", dotEXE, nombre_arch_corto, nombre_arch);

                }
            }
            arch_cmd.Close();
            conn.Clone();
        }

        private string RemoveAcentuation(string text)
        {
            return
                System.Web.HttpUtility.UrlDecode(
                    System.Web.HttpUtility.UrlEncode(
                        text, System.Text.Encoding.GetEncoding("iso-8859-7")));
        }

        private void guardaOper(ref List<Servicio> lServ, Servicio servicio, Operacion oper)
        {
            //existe serv?
            if (lServ.Exists(m => m.Id == servicio.Id))
            {
                var x = lServ.Find(m => m.Id == servicio.Id);
                x.Operaciones.Add(oper);
            }
            else
            {
                //crear serv
                servicio.Operaciones = new List<Operacion>();
                servicio.Operaciones.Add(oper);
                lServ.Add(servicio);
            }
            //retornar
        }

    }
}
