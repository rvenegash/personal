using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Xml;

namespace RevisaProyectosOSB_2
{
    class Program
    {
        static void Main(string[] args)
        {
            LeeProxysFront();

            LeeProxysBack();
        }

        static void LeeProxysFront()
        {
            Console.WriteLine("Leyendo todos los proys en Front");
            string carpeta = ConfigurationManager.AppSettings["OSBFront-BaseFolder"];
            string filtro = "proxy";

            string informe = ConfigurationManager.AppSettings["CarpetaInforme"];
            StreamWriter archivo = new StreamWriter(informe + "informe-front.txt");

            LeeCarpetaRecursiva(carpeta, filtro, archivo);

            archivo.Close();
        }

        static void LeeProxysBack()
        {
            Console.WriteLine("Leyendo todos los proys en Back");
            string carpeta = ConfigurationManager.AppSettings["OSBBack-BaseFolder"];
            string filtro = "proxy";

            string informe = ConfigurationManager.AppSettings["CarpetaInforme"];
            StreamWriter archivo = new StreamWriter(informe + "informe-back.txt");

            LeeCarpetaRecursiva(carpeta, filtro, archivo);

            archivo.Close();
        }

        static void LeeCarpetaRecursiva(string carpeta, string filtro, StreamWriter log)
        {
            Console.WriteLine("Revisando carpeta: " + carpeta);
            var list = Directory.GetDirectories(carpeta);
            foreach (var item in list)
            {
                var listF = Directory.GetFiles(item, "*." + filtro);
                foreach (var itemF in listF)
                {
                    try
                    {
                        AnalizaProxy(itemF, log);
                    }
                    catch
                    {
                        log.WriteLine("{0}\t{1}", "---- ERROR ----", itemF);
                        Console.WriteLine("Error revisando : ", itemF);
                    }
                }

                Console.WriteLine("Revisando sub carpetas ");
                LeeCarpetaRecursiva(item, filtro, log);
            }
        }

        private static void AnalizaProxy(string archivo, StreamWriter log)
        {
            Console.WriteLine("revisando proxy: " + archivo);

            // archivo.WriteLine(item);
            var url_proxy = "";

            XmlDocument doc = new XmlDocument();
            doc.Load(archivo);

            XmlNodeList bdRoot = doc.GetElementsByTagName("xml-fragment");
            foreach (XmlNode nodeP in bdRoot)
            {
                XmlElement subsElement = (XmlElement)nodeP;
                XmlNodeList stepsSE = subsElement.ChildNodes;

                foreach (XmlNode nodeStep in stepsSE)
                {

                    //RevisaNodo(nodeStep, fio.Name, folderName);
                    switch (nodeStep.LocalName)
                    {
                        //case "coreEntry":
                        //    leeCoreEntry(nodeStep);
                        //    break;
                        case "endpointConfig":
                            url_proxy = leeEndpointConfig(nodeStep);
                            break;
                        //case "router":
                        //    leeRouter(nodeStep);
                        //    break;
                        default:
                            break;
                    }
                }
            }

            log.WriteLine("{0}\t{1}", url_proxy, archivo);
        }

        private static string leeEndpointConfig(XmlNode nodeStep)
        {
            var res = "";
            foreach (XmlNode nodoHijo in nodeStep.ChildNodes)
            {
                switch (nodoHijo.LocalName)
                {
                    case "URI":
                        res = nodoHijo.ChildNodes[0].InnerText;

                        break;
                    default:
                        break;
                }
            }
            return res;
        }
    }
}
