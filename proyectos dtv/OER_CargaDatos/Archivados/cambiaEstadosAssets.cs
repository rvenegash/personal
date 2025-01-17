using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace OER_CargaDatos
{
    class cambiaEstadosAssets
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
            MessageQueue = 50007,
            BusinessEntity = 50009,
            Functionality = 50102
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

        static void MainEstadosAssets(string[] args)
        {
            Console.WriteLine("Creando servicio...");
            ws = new OerProd.FlashlineRegistryService();

            Console.WriteLine("Autenticando");
            token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);

            //bajar todos los assets en estado Unsubmitted
            Console.WriteLine("Leyendo assets");

            //recorrerlos y Submit
            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = (int)tipoAsset.Functionality;
            var serviceQ = ws.assetQuerySummary(token, criteria);
            foreach (var item in serviceQ)
            {
                var lAssetCriteria = new OER_CargaDatos.OerProd.AssetCriteria();
                lAssetCriteria.IDCriteria = item.ID;
                var lKeyValuePair = ws.assetEvaluate(token, lAssetCriteria, "Registration Status");
                if (lKeyValuePair.value.Equals("10")) //Unsubmitted
                {
                    Console.WriteLine("Submit : {0}, {1}", item.ID, item.name);
                    ws.assetSubmit(token, item.ID);
                }
                if (lKeyValuePair.value.Equals("51")) //Submitted
                {
                    Console.WriteLine("Accept : {0}, {1}", item.ID, item.name);
                    ws.assetAccept(token, item.ID);
                }
                if (lKeyValuePair.value.Equals("52")) //Accepted
                {
                    Console.WriteLine("Register : {0}, {1}", item.ID, item.name);
                    ws.assetRegister(token, item.ID);
                }
            }

            //fin
            Console.WriteLine("Terminado...");
            Console.ReadLine();
        }
    }
}
