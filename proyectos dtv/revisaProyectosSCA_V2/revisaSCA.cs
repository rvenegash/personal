
using revisaProyectosSCA_V2.Clases;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;

namespace revisaProyectosSCA_V2
{
    public class revisaSCA
    {
        List<string> keywords = new List<string> { "from", "bpelx:from", "bpelx:to", "to", "receive", "reply", "bpelx:target", "bpelx:validate", "invoke", "case", "bpelx:flowN", "throw", "catch", "bpelx:exec", "flow", "pick" };

        string operation_name;
        string service_name;
        string my_port_type;
        string nombre_archC = "";
        long operation_id;

        StreamWriter arch;
        StreamReader archFolders;
        StreamWriter archC;

        //List<PairKeyValue> lServices;
        List<String> bpels;
        List<JCA> lJca;
        List<JCA> lRefJca;

        public revisaSCA()
        {
        }

        public void procesar()
        {
            dataHelper.buildConn();

            revisaProyectosCarpetas();
        }

        private void revisaProyectosCarpetas()
        {
            archFolders = new StreamReader(ConfigurationManager.AppSettings["FoldersToParse"]);

            var nombre_arch = string.Format("revision-{0}.csv", DateTime.Now.ToString("yyyy-M-dd--HH-mm-ss"));
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            nombre_archC = string.Format("revision-{0}-composite.csv", DateTime.Now.ToString("yyyy-M-dd--HH-mm-ss"));

            dataHelper.borraTodoBD();

            try
            {
                while (archFolders.Peek() >= 0)
                {
                    service_name = "";

                    var linea = archFolders.ReadLine().Trim();

                    if (linea.StartsWith("#") || linea.Equals(""))
                    {
                        //saltar fila
                    }
                    else
                    {
                        bpels = new List<string>();

                        arch.WriteLine("----------------------------------------");

                        //abrir archivo composite.xml para obtener el nombre del servicio
                        //obtener lista de componentes bpel a revisar
                        var port_type = "";
                        lJca = new List<JCA>();
                        lRefJca = new List<JCA>();
                        try
                        {
                            service_name = parseComposite(linea + Path.DirectorySeparatorChar + "composite.xml", linea, out port_type);
                            if (service_name != "")
                            {
                                dataHelper.grabarService(service_name, port_type, linea);

                                //parsear cada bpel
                                foreach (var bpel in bpels)
                                {
                                    parseBpel(linea + Path.DirectorySeparatorChar + bpel, linea);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            arch.WriteLine("ex:{0}, {1}", linea, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            arch.Close();
            archFolders.Close();

            Console.WriteLine("Terminado!");

        }

        private string parseComposite(String file_name, string folder_name, out string port_type)
        {
            var res = "";
            port_type = "";
            Console.WriteLine(file_name);

            XmlDocument doc = new XmlDocument();
            doc.Load(file_name);

            XmlNodeList bdProcess = doc.GetElementsByTagName("composite");
            foreach (XmlNode nodeP in bdProcess)
            {
                XmlElement subsElement = (XmlElement)nodeP;
                XmlNodeList stepsSE = subsElement.ChildNodes;

                foreach (XmlNode nodeStep in stepsSE)
                {
                    switch (nodeStep.Name)
                    {
                        case "service":
                            if (res.Equals(""))
                            {
                                res = nodeStep.Attributes["name"].InnerText;
                                var wsdl = "";
                                try
                                {
                                    wsdl = nodeStep.Attributes["ui:wsdlLocation"].InnerText;
                                }
                                catch (Exception ex)
                                {
                                }

                                foreach (XmlNode nodoHijo in nodeStep.ChildNodes)
                                {
                                    if (nodoHijo.Name == "interface.wsdl")
                                    {
                                        var _interface = nodoHijo.Attributes["interface"].InnerText;
                                        port_type = _interface.Substring(_interface.IndexOf('(') + 1, _interface.IndexOf(')') - _interface.IndexOf('(') - 1);
                                        lineaArchivoC(string.Format("{0}\t{1}\t{2}\t{3}", file_name, res, wsdl, _interface));
                                    }
                                }
                                //arch.WriteLine("{0}\t{1}", file_name, res);
                            }

                            var oJca = new JCA() { name = nodeStep.Attributes["name"].InnerText, wsdlLocation = (nodeStep.Attributes["ui:wsdlLocation"] == null ? "" : nodeStep.Attributes["ui:wsdlLocation"].InnerText), config = "" };
                            foreach (XmlNode nodoHijo in nodeStep.ChildNodes)
                            {
                                if (nodoHijo.Name == "binding.jca")
                                {
                                    oJca.config = nodoHijo.Attributes["config"].Value;

                                    if (oJca.config != "")
                                    {
                                        XmlDocument docJca = new XmlDocument();
                                        docJca.Load(folder_name + Path.DirectorySeparatorChar + oJca.config);
                                        XmlNodeList bdJca = docJca.GetElementsByTagName("adapter-config");
                                        foreach (XmlNode nodeAC in bdJca)
                                        {
                                            XmlElement subsElementAC = (XmlElement)nodeAC;
                                            XmlNodeList stepsAC = subsElementAC.ChildNodes;

                                            foreach (XmlNode nodeStepAC in stepsAC)
                                            {
                                                if (nodeStepAC.Name == "connection-factory" && nodeStepAC.Attributes["location"] != null)
                                                {
                                                    oJca.cdLocation = nodeStepAC.Attributes["location"].Value;
                                                }
                                                if (nodeStepAC.Name == "endpoint-activation")
                                                {
                                                    //leer interaction - spec
                                                    XmlElement subsProp = (XmlElement)nodeStepAC;

                                                    foreach (XmlNode nodeProp in subsProp.ChildNodes[0].ChildNodes)
                                                    {
                                                        if (nodeProp.Attributes["name"] != null)
                                                        {
                                                            if (nodeProp.Attributes["name"].Value == "DestinationName" || nodeProp.Attributes["name"].Value == "DescriptorName")
                                                            {
                                                                oJca.DestinationName = nodeProp.Attributes["value"] != null ? nodeProp.Attributes["value"].Value : "";
                                                            }
                                                            if (nodeProp.Attributes["name"].Value == "MessageSelector")
                                                            {
                                                                oJca.MessageSelector = nodeProp.Attributes["value"] != null ? nodeProp.Attributes["value"].Value : "";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            if (oJca.config != "")
                            {
                                lJca.Add(oJca);
                            }
                            break;

                        case "component":
                            if (nodeStep.HasChildNodes)
                            {
                                foreach (XmlNode nodoHijo in nodeStep.ChildNodes)
                                {
                                    switch (nodoHijo.Name)
                                    {
                                        case "implementation.bpel":
                                            var a = nodoHijo.Attributes["src"].InnerText;
                                            bpels.Add(a);
                                            arch.WriteLine("{0}\t{1}\t{2}", file_name, service_name, a);

                                            break;

                                        case "business-events":
                                            var fio = new System.IO.DirectoryInfo(folder_name);
                                            res = fio.Name.Replace("SCA", "");

                                            foreach (XmlNode nodoBE in nodoHijo.ChildNodes)
                                            {
                                                switch (nodoBE.Name)
                                                {
                                                    case "subscribe":
                                                        port_type = nodoBE.Attributes["name"].InnerText;
                                                        port_type = res + "." + (port_type.Contains(":") ? port_type.Remove(0, port_type.IndexOf(":") + 1) : port_type);
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            break;

                        case "reference":
                            var wsdlRef = nodeStep.Attributes["ui:wsdlLocation"] == null ? "" : nodeStep.Attributes["ui:wsdlLocation"].InnerText;
                            if (wsdlRef != "")
                            {

                                var oJcaRef = new JCA() { name = nodeStep.Attributes["name"].InnerText, wsdlLocation = wsdlRef, config = "" };
                                foreach (XmlNode nodoHijo in nodeStep.ChildNodes)
                                {
                                    if (nodoHijo.Name == "binding.jca")
                                    {
                                        oJcaRef.config = nodoHijo.Attributes["config"].Value;
                                    }
                                }
                                if (oJcaRef.config != "")
                                {
                                    lRefJca.Add(oJcaRef);
                                }

                            }

                            break;

                        default:
                            break;
                    }
                }
            }

            return res;
        }

        private void lineaArchivoC(string linea)
        {
            archC = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_archC, true);
            archC.WriteLine(linea);
            archC.Close();
        }

        bool onMessageLeido;
        bool pickLeido;
        private void parseBpel(String file_name, string folderName)
        {
            if (System.IO.File.Exists(file_name))
            {
                var fio = new System.IO.FileInfo(file_name);

                //arch.WriteLine("{0}", file_name);

                operation_name = "";
                my_port_type = "";
                onMessageLeido = false;
                operation_id = 0;
                pickLeido = false;

                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(file_name);
                }
                catch (Exception ex)
                {
                    arch.WriteLine("ARCHIVO CON XML INVALIDO!!  : {0}", file_name);
                    return;
                }

                XmlNodeList bdProcess = doc.GetElementsByTagName("process");
                foreach (XmlNode nodeP in bdProcess)
                {
                    XmlElement subsElement = (XmlElement)nodeP;
                    XmlNodeList stepsSE = subsElement.ChildNodes;

                    foreach (XmlNode nodeStep in stepsSE)
                    {
                        RevisaNodo(nodeStep, fio.Name, folderName, file_name);
                    }
                }
            }
            else
            {
                arch.WriteLine("ERROR: ARCHIVO NO EXISTE!!  : {0}", file_name);
            }
        }

        private void RevisaNodo(XmlNode nodo, string file_name, string folderName, string archOrig)
        {
            switch (nodo.Name)
            {
                case "to":
                case "from":
                case "variable":
                case "empty":
                case "bpelx:to":
                case "bpelx:from":
                case "invoke":
                case "reply":
                case "bpelx:target":
                case "bpelx:validate":

                case "forEach":
                case "variables":
                case "sequence":
                case "partnerLink":
                case "partnerLinks":
                case "while":
                case "switch":
                case "if":
                case "elseif":
                case "else":
                case "case":
                case "otherwise":
                case "scope":
                case "receive":
                case "copy":
                case "assign":
                case "faultHandlers":
                case "catch":
                case "catchAll":
                case "bpelx:exec":
                case "bpelx:append":
                case "bpelx:remove":
                case "bpelx:flowN":
                case "flow":
                case "pick":
                case "onMessage":

                    if (nodo.Name == "pick")
                    {
                        pickLeido = true;
                    }
                    if (nodo.Name == "receive")
                    {
                        if (nodo.Attributes["operation"] != null)
                        {
                            operation_name = nodo.Attributes["operation"].InnerText;
                            my_port_type = nodo.Attributes["portType"].InnerText;

                            //buscar operación y servicio invocado
                            operation_id = dataHelper.grabaOperacion(operation_name, service_name, file_name, folderName, my_port_type, operation_id, service_name);
                        }

                        if (nodo.Attributes["bpelx:outputProperty"] != null)
                        {
                        }

                        if (nodo.Attributes["bpelx:eventName"] != null)
                        {
                            operation_name = nodo.Attributes["bpelx:eventName"].InnerText;
                            operation_name = (operation_name.Contains(":") ? operation_name.Remove(0, operation_name.IndexOf(":") + 1) : operation_name);

                            var fio = new System.IO.DirectoryInfo(folderName);
                            var folder = fio.Name.Replace("SCA", "");

                            my_port_type = folder + "." + operation_name;

                            //buscar operación y servicio invocado
                            operation_id = dataHelper.grabaOperacion(operation_name, service_name, file_name, folderName, my_port_type, operation_id, service_name);
                        }
                    }

                    if (pickLeido && nodo.Name == "onMessage")
                    {
                        if (operation_id == 0)
                        {
                            if (folderName.Contains("crm-assurance"))
                            {
                                operation_name = nodo.Attributes["operation"].InnerText;
                                my_port_type = nodo.Attributes["portType"].InnerText;
                            }
                            else
                            {
                                operation_name = Path.GetFileNameWithoutExtension(file_name);
                                my_port_type = service_name; // nodo.Attributes["portType"].InnerText;
                            }

                            //buscar operación y servicio invocado
                            operation_id = dataHelper.grabaOperacion(operation_name, service_name, file_name, folderName, my_port_type, operation_id, service_name);

                        }
                        if (operation_id > 0)
                        {
                            var pl = nodo.Attributes["partnerLink"].InnerText;

                            var ljca = lJca.Find(m => m.name == pl);
                            if (ljca != null)
                            {
                                var operation_invoked = ljca.DestinationName;
                                var partner_link = "";
                                var port_type = ljca.cdLocation;
                                dataHelper.grabaRelacionQueue(operation_id, operation_name, service_name, my_port_type, operation_invoked, partner_link, port_type, folderName, file_name);
                            }
                        }
                    }
                    if (nodo.HasChildNodes)
                    {
                        foreach (XmlNode nodoHijo in nodo.ChildNodes)
                        {
                            RevisaNodo(nodoHijo, file_name, folderName, archOrig);
                        }
                    }

                    if (nodo.Name == "invoke" && operation_name == "")
                    {
                        // casos donde el invoke va en fauls antes de obtener el nombre de la operacion
                        XmlDocument doc2 = new XmlDocument();
                        doc2.Load(archOrig); // folderName + Path.DirectorySeparatorChar + file_name);

                        var nombre2 = "";
                        XmlNodeList bdProcess = doc2.GetElementsByTagName("process");
                        foreach (XmlNode nodeP in bdProcess)
                        {
                            XmlElement subsElement = (XmlElement)nodeP;
                            XmlNodeList stepsSE = subsElement.ChildNodes;

                            foreach (XmlNode nodeStep in stepsSE)
                            {
                                RevisaNodo2(nodeStep, ref nombre2);
                            }
                        }
                        if (nombre2 != "")
                        {
                            operation_name = nombre2;
                        }
                    }

                    if (nodo.Name == "invoke" && operation_name != "")
                    {
                        var operation_invoked = "";
                        var partner_link = "";
                        var port_type = "";
                        if (nodo.Attributes["operation"] != null)
                        {
                            operation_invoked = nodo.Attributes["operation"].InnerText;
                            partner_link = nodo.Attributes["partnerLink"].InnerText;
                            port_type = nodo.Attributes["portType"].InnerText;
                        }
                        if (nodo.Attributes["bpelx:eventName"] != null)
                        {
                            operation_invoked = nodo.Attributes["bpelx:eventName"].InnerText;
                        }

                        if (nodo.Attributes["outputVariable"] == null && partner_link != "") //no hay variable de output, es cola??
                        {
                            var type = "";
                            var queue = "";
                            queue = buscaNombreColaBpel(folderName, file_name, partner_link, port_type, operation_invoked, out type);
                            if (queue != "")
                            {
                                operation_invoked = queue;
                                port_type = type;
                                //operation_invoked, partner_link, port_type
                            }
                        }
                        arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", file_name, my_port_type, operation_name, port_type, operation_invoked);

                        dataHelper.grabaRelacion(operation_id, operation_name, service_name, my_port_type, operation_invoked, partner_link, port_type, folderName, file_name);
                    }

                    break;
                default:
                    break;
            }
        }

        private void RevisaNodo2(XmlNode nodo, ref string nombre2)
        {
            switch (nodo.Name)
            {
                case "to":
                case "from":
                case "sequence":
                case "scope":
                case "receive":
                case "flow":
                case "pick":
                case "onMessage":

                    if (nodo.Name == "receive" || nodo.Name == "onMessage")
                    {
                        if (nodo.Attributes["operation"] != null)
                        {
                            nombre2 = operation_name = nodo.Attributes["operation"].InnerText;
                            my_port_type = nodo.Attributes["portType"].InnerText;
                        }
                    }
                    if (nodo.HasChildNodes)
                    {
                        foreach (XmlNode nodoHijo in nodo.ChildNodes)
                        {

                            RevisaNodo2(nodoHijo, ref nombre2);
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private string buscaNombreColaBpel(string folderName, string archivo, string partnerLink, string portType, string operacion, out string TYPE)
        {
            var res = "";
            TYPE = "";
            //buscar propio .componentType
            var ctf = Path.ChangeExtension(folderName + Path.DirectorySeparatorChar + archivo, ".componentType");
            if (File.Exists(ctf))
            {
                //buscar partnerLink
                XmlDocument doc2 = new XmlDocument();
                doc2.Load(ctf);

                var wsdl = "";
                var wsdl2 = "";
                var jca = "";

                var plref = lRefJca.Find(m => m.name == partnerLink);
                if (plref != null)
                {
                    wsdl = folderName + Path.DirectorySeparatorChar + plref.wsdlLocation;
                    wsdl2 = plref.wsdlLocation;

                    jca = folderName + Path.DirectorySeparatorChar + plref.config;
                }
                if (wsdl == "")
                {
                    XmlNodeList bdCT = doc2.GetElementsByTagName("componentType");
                    foreach (XmlNode nodeP in bdCT)
                    {
                        XmlElement subsElement = (XmlElement)nodeP;
                        XmlNodeList stepsSE = subsElement.ChildNodes;

                        foreach (XmlNode nodeStep in stepsSE)
                        {
                            if (nodeStep.Name == "reference" && nodeStep.Attributes["name"].Value == partnerLink && nodeStep.Attributes["ui:wsdlLocation"] != null)
                            {
                                wsdl = folderName + Path.DirectorySeparatorChar + nodeStep.Attributes["ui:wsdlLocation"].Value;
                                wsdl2 = nodeStep.Attributes["ui:wsdlLocation"].Value;
                            }
                        }
                    }
                }
                if (wsdl != "" && jca == "" && !wsdl2.Contains("oramds:"))
                {
                    var ljca = lRefJca.Find(m => m.wsdlLocation == wsdl2);
                    if (ljca != null)
                    {
                        jca = folderName + Path.DirectorySeparatorChar + ljca.config;
                    }
                    else
                    {
                        //buscar jca asociado
                        if (File.Exists(wsdl))
                        {
                            XmlDocument docWsdl = new XmlDocument();
                            docWsdl.Load(wsdl);
                            var xBind = docWsdl.FirstChild;
                            if (xBind.Name == "binding.jca")
                            {
                                jca = folderName + Path.DirectorySeparatorChar + xBind.Value;
                            }
                        }
                    }
                }
                if (jca != "" && !jca.Contains("oramds:"))
                {
                    XmlDocument docJca = new XmlDocument();
                    docJca.Load(jca);
                    XmlNodeList bdJca = docJca.GetElementsByTagName("adapter-config");
                    foreach (XmlNode nodeP in bdJca)
                    {
                        XmlElement subsElement = (XmlElement)nodeP;
                        XmlNodeList stepsSE = subsElement.ChildNodes;

                        foreach (XmlNode nodeStep in stepsSE)
                        {
                            var portType2 = portType.Substring(portType.IndexOf(":") + 1);
                            if (nodeStep.Name == "connection-factory" && nodeStep.Attributes["location"] != null)
                            {
                                TYPE = nodeStep.Attributes["location"].Value;
                            }
                            if (nodeStep.Name == "endpoint-interaction" && nodeStep.Attributes["portType"].Value == portType2 && nodeStep.Attributes["operation"].Value == operacion)
                            {
                                //leer interaction - spec
                                XmlElement subsProp = (XmlElement)nodeStep;

                                foreach (XmlNode nodeProp in subsProp.ChildNodes[0].ChildNodes)
                                {
                                    if (nodeProp.Attributes["name"] != null)
                                    {
                                        if (nodeProp.Attributes["name"].Value == "DestinationName" || nodeProp.Attributes["name"].Value == "DescriptorName")
                                        {
                                            res = nodeProp.Attributes["value"] != null ? nodeProp.Attributes["value"].Value : "";
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                //leer jca y ponerlo en el nombre operacion, ejemplo "Produce_Message_jms/ArgentinaQueue"
            }

            return res;
        }




    }
}
