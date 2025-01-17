using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace OER_CargaDatos
{
    class exportaOperIBS
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

        static void Main1(string[] args)
        {
            Console.WriteLine("Creando servicio...");
            ws = new OerProd.FlashlineRegistryService();

            Console.WriteLine("Autenticando");
            token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);

            //bajar todos los assets en estado Unsubmitted
            Console.WriteLine("Leyendo assets");

            var swArchivo = new StreamWriter(System.Configuration.ConfigurationManager.AppSettings["Path"] + "operaciones_v62.csv");

            //recorrerlos y Submit
            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = (int)tipoAsset.Operation;
            criteria.versionCriteria = "6.2";
            var serviceQ = ws.assetQuerySummary(token, criteria);
            foreach (var item in serviceQ)
            {
                var asset = ws.assetRead(token, item.ID);
                var service_name = "";
                var operation_name = item.name;
                foreach (var rel in asset.relationshipTypes)
                {
                    if (rel.ID == (int)tipoRelation.Contains)
                    {
                        if (rel.primaryIDs.Length > 0)
                        {
                            //var assetR = ws.assetRead(token, rel.primaryIDs[0]);

                            var criteria2 = new OER_CargaDatos.OerProd.AssetCriteria();
                            criteria2.IDCriteria = rel.primaryIDs[0];
                            var serviceS = ws.assetQuerySummary(token, criteria2);
                            if (serviceS.Count() > 0)
                            {
                                service_name = serviceS.First().name;
                            }
                        }
                    }
                }
                Console.WriteLine(string.Format("{0}\t{1}", operation_name, service_name));
                swArchivo.WriteLine(string.Format("{0}\t{1}", operation_name, service_name));
            }
            swArchivo.Close();
            //din
            Console.WriteLine("Terminado...");
            Console.ReadLine();
        }
    }
}
