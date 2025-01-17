using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace ConsoleRevisaDiffICX_K2V
{
    class Program
    {
        static void Main(string[] args)
        {
            var carpeta = ConfigurationManager.AppSettings["ResponseFolder"];
            var clientes = ConfigurationManager.AppSettings["ArchivoClientes"];
            var log = ConfigurationManager.AppSettings["ArchivoLog"];

            var sr = new StreamReader(clientes);
            var sw = new StreamWriter(log);

            while (sr.Peek() >= 0)
            {
                var cliente = sr.ReadLine();
                if (!string.IsNullOrEmpty(cliente))
                {
                    var estIcx = "";
                    var tipIcx = "";

                    Console.WriteLine("{0}", cliente);
                    var srI = new StreamReader(carpeta + cliente + "-ibs.xml");
                    var xml = srI.ReadToEnd();
                    leeEstadoXml(xml, out estIcx, out tipIcx);

                    var estK2 = "";
                    var tipK2 = "";
                    var srK = new StreamReader(carpeta + cliente + "_k2.json");
                    var json = srK.ReadToEnd();
                    leeEstadoJson(json, out estK2, out tipK2);

                    if (estIcx != estK2 || tipIcx != tipK2)
                    {
                        Console.WriteLine(" DISTINTOS");
                        Console.WriteLine("Icx e:{0}, t:{0}", estIcx, tipIcx);
                        Console.WriteLine("K2v e:{0}, t:{0}", estK2, tipK2);
                        sw.WriteLine("{0}\tDISTINTOS\ticx e:{1}, t{2}\tk2 e:{3}, t:{4}", cliente, estIcx, tipIcx, estK2, tipK2);
                    }
                    else
                    {
                        Console.WriteLine("IGUALES  e:{0}, t:{0}", estIcx, tipIcx);
                        sw.WriteLine("{0}\tIGUALES\te:{1}, t{2} ", cliente, estIcx, tipIcx);
                    }
                }
            }

            sw.Close();
            Console.WriteLine("Fin");
            Console.ReadLine();
        }

        private static void leeEstadoJson(string sjson, out string estado, out string tipo)
        {
            JObject json = JObject.Parse(sjson);

            estado = json["data"]["GetCustomer360ViewResponse"]["GetCustomer360ViewResult"]["Customer"]["status"]["id"].ToString();
            tipo = json["data"]["GetCustomer360ViewResponse"]["GetCustomer360ViewResult"]["Customer"]["CategorizedBy"]["id"].ToString();

        }

        private static void leeEstadoXml(string xml, out string estado, out string tipo)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XPathNavigator navigator = doc.CreateNavigator();
            XPathExpression queryS = navigator.Compile("/soapenv:Envelope/env:Body/ns:GetCustomerResponse/crmsar-v1-0:GetCustomerResult/crmsar-v1-0:customer/commonbd-v1-0:status/commonbd-v1-0:id");
            XPathExpression queryT = navigator.Compile("/soapenv:Envelope/env:Body/ns:GetCustomerResponse/crmsar-v1-0:GetCustomerResult/crmsar-v1-0:customer/commonbd-v1-0:CategorizedBy/commonbd-v1-0:id");
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            manager.AddNamespace("env", "http://schemas.xmlsoap.org/soap/envelope/");
            manager.AddNamespace("ns", "http://directvla.com/schema/businessdomain/CRMSupportAndReadiness/v1-0");
            manager.AddNamespace("crmsar-v1-0", "http://directvla.com/schema/businessdomain/CRMSupportAndReadiness/v1-0");
            manager.AddNamespace("commonbd-v1-0", "http://directvla.com/schema/businessdomain/common/v1-0");
            queryS.SetContext(manager);
            queryT.SetContext(manager);

            estado = "";
            XPathNodeIterator nodesS = navigator.Select(queryS);
            if (nodesS.Count > 0)
            {
                while (nodesS.MoveNext())
                {
                    estado = nodesS.Current.Value;
                }
            }

            tipo = "";
            XPathNodeIterator nodesT = navigator.Select(queryT);
            if (nodesT.Count > 0)
            {
                while (nodesT.MoveNext())
                {
                    tipo = nodesT.Current.Value;
                }
            }

            return;
        }
    }
}
