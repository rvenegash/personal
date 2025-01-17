using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace OER_CargaDatos
{
    class cambiaProducingProject
    {
        enum tipoAsset
        {
            Service = 154,
            Endpoint = 169,
            Application = 158,
            Interface = 187,
            Operation = 50004,
            Country = 50006,
            DataCenter = 50005,
            MessageQueue = 50007
        }
        enum tipoRelation
        {
            Implements = 119,
            Invoked = 50000,
            Contains = 108,
            ReferencedBy = 118
        }
        enum estadoRegistro
        {
            Unsubmitted = 10,
            Submitted = 51,
            Accepted = 52,
            Registered = 13
        }
        static OerProd.FlashlineRegistryService ws;
        static OerProd.AuthToken token;
        static List<long> servicios;

        static void MainProdProj(string[] args)
        {
            //servicios
            //operaciones
            //colas
            //interface

            //ibs -> proyecto propio
            //
            Console.WriteLine("Creando servicio...");
            ws = new OerProd.FlashlineRegistryService();

            Console.WriteLine("Autenticando");
            token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);

            servicios = new List<long>();
            cargaServicios();

            //leerProjects();

            asignaProdProject();
        }

        private static void leerProjects()
        {
            Console.WriteLine("Leyendo proyectos");

            var criteria = new OerProd.ProjectCriteria();
            var serviceQ = ws.projectQuerySummary(token, criteria);
            foreach (var item in serviceQ)
            {
                Console.WriteLine("{0} - {1}", item.ID, item.name);

            }
        }

        private static void asignaProdProject()
        {
            Console.WriteLine("Leyendo assets");


            Console.WriteLine("Nombre de archivo:");
            var nom_arch = Console.ReadLine();
            if (nom_arch.Equals(""))
            {
                Console.WriteLine("Nombre de archivo no valido!!");

                return;
            }

            var sw = new System.IO.StreamWriter(ConfigurationManager.AppSettings["Path"] + nom_arch.Trim() + ".txt");

            //50001 - Argentina
            //50002 - Colombia
            //50000 - Common Project
            //50102 - IBS
            //50101 - Iniciativa SOA
            //50003 - Venezuela
            var proj_id = 50101;

            //recorrerlos y Submit
            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = (int)tipoAsset.Operation;
            //criteria.assetTypeCriteria = (int)tipoAsset.Service;
            //criteria.versionCriteria = "6.2";
            criteria.versionCriteria = "1.0";

            var serviceQ = ws.assetQuerySummary(token, criteria);
            foreach (var item in serviceQ)
            {
                var crear_prod_proj = true;
                var crear_prod_proj2 = false;
                var asset = ws.assetRead(token, item.ID);

                foreach (var rel in asset.relationshipTypes)
                {
                    if (rel.ID == (int)tipoRelation.Contains)
                    {
                        if (rel.primaryIDs.Any(p => servicios.Any(s => s == p)))
                        {
                            crear_prod_proj2 = true;
                        }
                    }
                }
                if (asset.producingProjectsIDs.Count() > 0)
                {
                    crear_prod_proj = (!asset.producingProjectsIDs.Contains(proj_id));
                }
                if (crear_prod_proj && crear_prod_proj2)
                {
                    var ultimo = asset.producingProjectsIDs.Length;
                    var paso = new long[asset.producingProjectsIDs.Length + 1];
                    Array.Copy(asset.producingProjectsIDs, paso, asset.producingProjectsIDs.Length);

                    paso[ultimo] = proj_id;
                    asset.producingProjectsIDs = paso;

                    Console.WriteLine("Actualizando {0} - {1} {2}", item.ID, item.name, item.version);
                    sw.WriteLine("Actualizando {0} - {1} {2}", item.ID, item.name, item.version);
                    var assU = ws.assetUpdate(token, asset);
                }
                else
                {
                    Console.WriteLine("YA actualizado {0} - {1} {2}", item.ID, item.name, item.version);
                    sw.WriteLine("YA actualizado {0} - {1} {2}", item.ID, item.name, item.version);
                }
            }
            sw.Close();
            Console.WriteLine("Terminado!");
            Console.ReadKey();
        }


        private static void cargaServicios()
        {
            servicios.Add(50068);
            servicios.Add(50807);
            servicios.Add(50806);
            servicios.Add(50808);
            servicios.Add(50809);
            servicios.Add(50810);
            servicios.Add(50811);
            servicios.Add(50357);
            servicios.Add(50012);
            servicios.Add(50930);
            servicios.Add(50932);
            servicios.Add(50929);
            servicios.Add(50071);
            servicios.Add(50931);
            servicios.Add(50005);
            servicios.Add(50072);
            servicios.Add(50801);
            servicios.Add(50107);
            servicios.Add(50083);
            servicios.Add(50020);
            servicios.Add(50087);
            servicios.Add(50089);
            servicios.Add(50021);
            servicios.Add(50506);
            servicios.Add(50557);
            servicios.Add(50707);
            servicios.Add(50518);
            servicios.Add(50093);
            servicios.Add(50516);
            servicios.Add(50096);
            servicios.Add(50596);
            servicios.Add(50099);
            servicios.Add(50101);
            servicios.Add(50102);
            servicios.Add(50711);
            servicios.Add(50103);
            servicios.Add(50348);
        }
    }
}
