using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace OER_CargaDatos
{
    class actualizaDatosROI
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

        static public void MainRoi2(string[] args)
        {
            Console.WriteLine("Creando servicio...");
            ws = new OerProd.FlashlineRegistryService();

            Console.WriteLine("Autenticando");
            token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);

            //bajar todos los assets en estado Unsubmitted
            Console.WriteLine("Leyendo assets");

            var sr = new StreamReader(ConfigurationManager.AppSettings["Path"] + "operaciones - metrics.csv");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["Path"] + "operaciones - metrics.log");

            //recorrer archivo
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split(';');

                var sOperacion = linea[0].Trim();
                var sB = "60";
                var sN = "0";

                //validar linea
                long operacionId = 0;
                if (Int64.TryParse(sOperacion, out operacionId))
                {
                    //buscar asset
                    Console.WriteLine("Buscando {0}", operacionId);
                    try
                    {
                        var tieneTdh = false;
                        var valorTdh = 0;
                        var asset_name = "";
                        var item = ws.assetReadXml(token, operacionId);
                        if (item != "")
                        {
                            var doc = new XmlDocument();
                            doc.LoadXml(item);

                            
                            XmlNodeList bdName = doc.GetElementsByTagName("name");
                            foreach (XmlNode nodeName in bdName)
                            {
                                asset_name = nodeName.InnerText;
                                break;
                            }
                            XmlNodeList bdCD = doc.GetElementsByTagName("custom-data");
                            foreach (XmlNode nodeP in bdCD)
                            {
                                XmlElement subsElement = (XmlElement)nodeP;
                                XmlNodeList stepsSE = subsElement.ChildNodes;

                                foreach (XmlNode nodeStep in stepsSE)
                                {
                                    if (nodeStep.Name == "total-development-hours--tdh-")
                                    {
                                        tieneTdh = true;
                                        valorTdh = Convert.ToInt32(nodeStep.InnerText) * 3;
                                        break;
                                    }
                                }
                            }
                        }

                        if (tieneTdh)
                        {
                              
/*
  <total-development-hours--tdh->480</total-development-hours--tdh-> 
  <production-investment--pinv->48</production-investment--pinv-> 
  <consumption-factor--cfac->10</consumption-factor--cfac-> 
  <predicted-number-of-annual-reuse-opportunities--n->0</predicted-number-of-annual-reuse-opportunities--n-> 
  <hourly-burden-rate--b->60</hourly-burden-rate--b-> 
*/
                            /*
                            var arrayPath = new string[3] { "total-development-hours--tdh-", "predicted-number-of-annual-reuse-opportunities--n-", "hourly-burden-rate--b-" };
                            var arrayValues = new string[3] { valorTdh.ToString(), sN, sB };
                            */
                            var arrayPath = new string[2] { "production-investment--pinv-", "consumption-factor--cfac-" };
                            var arrayValues = new string[2] { "10", "5" };

                            var assetU2 = ws.assetUpdateCustomDataStringArray(token, operacionId, arrayPath, arrayValues);
                            sw.WriteLine("{0}\t{1}\t{2}\t{3}", operacionId, valorTdh, sN, sB);

                            Console.WriteLine("Cambiado {0}, {1}", operacionId, asset_name);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            sr.Close();
            sw.Close();

            //fin
            Console.WriteLine("Terminado...");
            Console.ReadLine();
        }

        static void MainRoi(string[] args)
        {
            Console.WriteLine("Creando servicio...");
            ws = new OerProd.FlashlineRegistryService();

            Console.WriteLine("Autenticando");
            token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);

            //bajar todos los assets en estado Unsubmitted
            Console.WriteLine("Leyendo assets");

            var sr = new StreamReader(ConfigurationManager.AppSettings["PathROI"] + "operaciones - funcionality.csv");
            var sw = new StreamWriter(ConfigurationManager.AppSettings["PathROI"] + "operaciones - funcionality.log");

            //recorrer archivo
            while (sr.Peek() >= 0)
            {
                var linea = sr.ReadLine().Split(';');

                if (linea.Count() == 16)
                {
                    var sOperacion = linea[0].Trim();
                    var sOperacionN = linea[1].Trim();
                    var sTdh = linea[9].Trim();
                    var sCfac = linea[11].Trim();
                    var sPinv = linea[13].Trim();
                    var sB = linea[14].Trim();
                    var sN = linea[15].Trim();

                    //validar linea
                    long operacionId = 0;
                    if (Int64.TryParse(sOperacion, out operacionId))
                    {
                        //saltar los N/A
                        if (!sTdh.Equals("#N/A"))
                        {
                            //buscar asset
                            Console.WriteLine("Buscando {0}\t{1}", operacionId, sOperacionN);
                            try
                            {
                                //var assetX = ws.assetRead(token, operacionId);

                                var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
                                criteria.IDCriteria = operacionId;
                                var assetL = ws.assetQuerySummary(token, criteria);
                                if (assetL != null && assetL.Count() > 0)
                                {
                                    //actualizar asset
                                    var asset = assetL.First();
                                    //development-hours, estimated-time-to-use
                                    var arrayPath = new string[5] { "total-development-hours--tdh-", "production-investment--pinv-", "consumption-factor--cfac-", "predicted-number-of-annual-reuse-opportunities--n-", "hourly-burden-rate--b-" };
                                    var arrayValues = new string[5] { sTdh, sPinv, sCfac, sN, sB };

                                    var assetU2 = ws.assetUpdateCustomDataStringArray(token, asset.ID, arrayPath, arrayValues);
                                    sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", operacionId, sTdh, sPinv, sCfac, sN, sB);

                                    var tabs = ws.assetTabsRead(token, asset.ID);
                                    if (tabs != null)
                                    {
                                        var tabMetrics = tabs.Where(m => m.name == "Metrics");
                                        if (tabMetrics.Count() > 0)
                                        {
                                            var tab = tabMetrics.First();
                                            if (!tab.approved)
                                            {
                                                Console.WriteLine("aprobando...");
                                                ws.assetTabApprove(token, asset.ID, tab.id);

                                                sw.WriteLine("aprobando...");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            sr.Close();
            sw.Close();

            //fin
            Console.WriteLine("Terminado...");
            Console.ReadLine();
        }
    }
}
