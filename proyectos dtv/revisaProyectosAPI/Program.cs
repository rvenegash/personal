using Microsoft.Extensions.Configuration;
using revisaProyectosAPI.Clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace revisaProyectosAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            //package manager  : install-package Microsoft.Extensions.Configuration.Json
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            var cs = config.GetConnectionString("DefaultConnection");

            var folder = config.GetSection("MySettings").GetSection("APIFolder").Value;

            string filtro = "proxy";

            string informe = config.GetSection("MySettings").GetSection("CarpetaInforme").Value;
            StreamWriter archivo = new StreamWriter(informe + "informe-apis.txt");

            foreach (var item in Directory.EnumerateDirectories(folder))
            {
                if (!item.StartsWith("."))
                {
                    LeeCarpetaRecursiva(item, filtro, archivo);

                    Console.WriteLine(item);
                }

            }


            archivo.Close();
            //Console.ReadKey();
            Console.WriteLine("fin...");
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
                    catch (Exception ex)
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

            var url_proxy = "";

            XmlDocument doc = new XmlDocument();
            doc.Load(archivo);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("ser", "http://www.bea.com/wli/sb/services");
            nsmgr.AddNamespace("con", "http://www.bea.com/wli/sb/pipeline/config");
            nsmgr.AddNamespace("con1", "http://www.bea.com/wli/sb/stages/transform/config");
            nsmgr.AddNamespace("con2", "http://www.bea.com/wli/sb/stages/config");
            nsmgr.AddNamespace("con3", "http://www.bea.com/wli/sb/stages/routing/config");
            nsmgr.AddNamespace("tran", "http://www.bea.com/wli/sb/transports");
            nsmgr.AddNamespace("env", "http://www.bea.com/wli/config/env");

            string archPL = "";
            //proxy
            XmlNodeList bdRoot = doc.GetElementsByTagName("proxyServiceEntry", "http://www.bea.com/wli/sb/services");
            foreach (XmlNode nodeP in bdRoot)
            {
                XmlElement subsElement = (XmlElement)nodeP;
                XmlNodeList stepsSE = subsElement.ChildNodes;

                foreach (XmlNode nodeStep in stepsSE)
                {
                    switch (nodeStep.LocalName)
                    {
                        case "coreEntry":
                            if (nodeStep.SelectSingleNode("ser:invoke", nsmgr) != null)
                            {
                                var npl = nodeStep.SelectSingleNode("ser:invoke", nsmgr).Attributes["ref"].InnerText;
                                archPL = npl.Substring(npl.LastIndexOf('/') + 1);
                            }
                            break;
                        case "endpointConfig":
                            if (nodeStep.SelectSingleNode("tran:URI/env:value", nsmgr) != null)
                            {
                                url_proxy = nodeStep.SelectSingleNode("tran:URI/env:value", nsmgr).InnerText;
                            }
                            else
                            {
                                url_proxy = "--NO ENCONTRADO--";
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            //pipeline
            if (archPL == "") { archivo = Path.ChangeExtension(archivo, "pipeline"); }
            else { archivo = Path.GetDirectoryName(archivo) + Path.DirectorySeparatorChar + archPL + ".pipeline"; }

            Console.WriteLine("revisando pipeline: " + archivo);
            doc = new XmlDocument();
            doc.Load(archivo);

            bdRoot = doc.GetElementsByTagName("pipelineEntry", "http://www.bea.com/wli/sb/pipeline/config");
            foreach (XmlNode nodeP in bdRoot)
            {
                XmlElement subsElement = (XmlElement)nodeP;
                XmlNodeList stepsSE = subsElement.ChildNodes;

                foreach (XmlNode nodeStep in stepsSE)
                {
                    switch (nodeStep.LocalName)
                    {
                        case "router":
                            var lstPipelines = new List<Pipeline>();
                            foreach (XmlNode item in nodeStep.SelectNodes("con:pipeline", nsmgr))
                            {
                                var nombre = item.Attributes["name"].InnerText;
                                foreach (XmlNode item2 in item.SelectNodes("con:stage/con:actions/con1:wsCallout", nsmgr))
                                {
                                    var p = new Pipeline() { name = nombre };
                                    foreach (XmlNode item3 in item2.SelectNodes("con1:requestTransform/con1:assign/con1:expr/con2:xqueryTransform/con2:param", nsmgr))
                                    {
                                        if (item3.Attributes["name"].InnerText == "SYSTEM_ID")
                                        {
                                            p.systemId = item3.InnerText;
                                        }
                                    }
                                    foreach (XmlNode item3 in item2.SelectNodes("con1:responseTransform/con1:assign/con1:expr", nsmgr))
                                    {
                                        p.route = item3.InnerText;
                                    }
                                    if (!(p.route is null)) lstPipelines.Add(p);
                                }
                            }

                            var r = leeFlow(nodeStep.SelectSingleNode("con:flow", nsmgr), nsmgr, lstPipelines);

                            foreach (var item in r)
                            {
                                foreach (var item2 in item.operaciones)
                                {
                                    log.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", url_proxy, archivo, item.metodo, item.nombre, item2.operation, item2.service, item2.route, item2.systemId);
                                }
                            }

                            break;
                        //case "flow":
                        //    leeFlow(nodeStep, nsmgr);
                        //    break;
                        default:
                            break;
                    }
                }
            }

        }

        private static List<Recurso> leeFlow(XmlNode nodeStep, XmlNamespaceManager nsmgr, List<Pipeline> lstPipelines)
        {
            var res = new List<Recurso>();
            if (nodeStep != null)
            {
                foreach (XmlNode nodeP in nodeStep.SelectNodes("con:branch-node/con:branch-table", nsmgr))
                {
                    XmlElement subsElement = (XmlElement)nodeP;
                    XmlNodeList stepsSE = subsElement.ChildNodes;
                    foreach (XmlNode nodo1 in stepsSE)
                    {
                        if (nodo1.LocalName.Equals("branch") || nodo1.LocalName.Equals("default-branch"))
                        {
                            string nombretemp = "";
                            if (nodo1.SelectSingleNode("con:value", nsmgr) is null)
                            {
                                string path = "";
                                string verb = "";
                                foreach (XmlNode item in nodo1.SelectNodes("con:rest-values", nsmgr))
                                {
                                    if (item.SelectSingleNode("con:path", nsmgr) != null)
                                    {
                                        path = item.SelectSingleNode("con:path", nsmgr).InnerText;
                                    }
                                    if (item.SelectSingleNode("con:verb", nsmgr) != null)
                                    {
                                        verb = item.SelectSingleNode("con:verb", nsmgr).InnerText;
                                    }
                                }
                                nombretemp = verb + "." + path;
                            }
                            else
                            { nombretemp = nodo1.SelectSingleNode("con:value", nsmgr).InnerText.Replace("'", "").Replace("\"", ""); }

                            var recurso = new Recurso()
                            {
                                nombre = nombretemp.IndexOf(".") > 0 ? nombretemp.Substring(nombretemp.IndexOf(".") + 1) : nombretemp,
                                metodo = nombretemp.IndexOf(".") > 0 ? nombretemp.Substring(0, nombretemp.IndexOf(".")) : "",
                                operaciones = new List<Entidad>()
                            };

                            var requestName = "";
                            foreach (XmlNode item in nodo1.SelectNodes("con:flow/con:pipeline-node", nsmgr))
                            {
                                if (item.SelectSingleNode("con:request", nsmgr) != null)
                                {
                                    requestName = item.SelectSingleNode("con:request", nsmgr).InnerText;
                                }
                            }

                            foreach (XmlNode item in nodo1.SelectNodes("con:flow/con:route-node/con:actions/con3:route", nsmgr))
                            {
                                var operacion = new Entidad() { operation = "--NO ENCONTRADO--", service = "--NO ENCONTRADO--" };

                                if (item.SelectSingleNode("con3:operation", nsmgr) != null)
                                {
                                    operacion.operation = item.SelectSingleNode("con3:operation", nsmgr).InnerText;
                                }
                                else
                                {
                                    if (item.SelectSingleNode("con3:outboundTransform/con1:transport-headers/con1:header[@name='SOAPAction']", nsmgr) != null)
                                    {
                                        operacion.operation = item.SelectSingleNode("con3:outboundTransform/con1:transport-headers/con1:header[@name='SOAPAction']", nsmgr).InnerText;
                                    }
                                }

                                if (item.SelectSingleNode("con3:service", nsmgr) != null)
                                {
                                    operacion.service = item.SelectSingleNode("con3:service", nsmgr).Attributes["ref"].Value;
                                }
                                if (requestName != "")
                                {
                                    var pip = lstPipelines.Where(m => m.name == requestName).FirstOrDefault();
                                    if (pip != null)
                                    {
                                        operacion.route = pip.route;
                                        operacion.systemId = pip.systemId;
                                    }
                                }
                                recurso.operaciones.Add(operacion);
                            }
                            res.Add(recurso);
                        }
                    }
                }

                foreach (XmlNode nodeP in nodeStep.SelectNodes("con:route-node/con:actions/con3:route", nsmgr))
                {
                    XmlElement subsElement = (XmlElement)nodeP;
                    XmlNodeList stepsSE = subsElement.ChildNodes;
                    foreach (XmlNode nodo1 in stepsSE)
                    {
                        string nombretemp = "";

                        var operacion = new Entidad() { operation = "--NO ENCONTRADO--", service = "--NO ENCONTRADO--" };
                        var recurso = new Recurso()
                        {
                            nombre = nombretemp.IndexOf(".") > 0 ? nombretemp.Substring(nombretemp.IndexOf(".") + 1) : nombretemp,
                            metodo = nombretemp.IndexOf(".") > 0 ? nombretemp.Substring(0, nombretemp.IndexOf(".")) : "",
                            operaciones = new List<Entidad>()
                        };

                        if (nodo1.Name == "con3:service")
                        {
                            operacion.service = nodo1.Attributes["ref"].Value;

                            recurso.operaciones.Add(operacion);

                            res.Add(recurso);
                        }
                    }
                }
            }
            return res;
        }
    }
}
