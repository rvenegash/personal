using MySql.Data.MySqlClient;
using revisaProyectosSCA.Clases;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace revisaProyectosSCA
{
    public partial class Form3 : Form
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

        MySqlConnection conn;

        //List<PairKeyValue> lServices;
        List<String> bpels;
        List<JCA> lJca;
        List<JCA> lRefJca;

        enumEjecutar ejecutar;

        enum enumEjecutar
        {
            No,
            Parte1,
            Parte2,
            Parte3
        }

        #region Eventos Form
        public Form3()
        {
            InitializeComponent();

            ejecutar = enumEjecutar.No;
        }
        public Form3(string Parametro)
        {
            InitializeComponent();

            switch (Parametro)
            {
                case "/run1":
                    ejecutar = enumEjecutar.Parte1;
                    break;
                case "/run2":
                    ejecutar = enumEjecutar.Parte2;
                    break;
                case "/run3":
                    ejecutar = enumEjecutar.Parte3;
                    break;
                default:
                    ejecutar = enumEjecutar.No;
                    break;
            }
            // ejecutar = (Parametro == "/run1" ? enumEjecutar.Parte1 : (Parametro == "/run2" ? enumEjecutar.Parte2 : enumEjecutar.No));
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            var mysqldb = ConfigurationManager.AppSettings["MySQL160"];
            conn = new MySqlConnection(mysqldb);
            conn.Open();

        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            conn.Close();
        }

        private void Form3_Activated(object sender, EventArgs e)
        {
            if (ejecutar == enumEjecutar.Parte1)
            {
                button1_Click(null, new EventArgs());
            }
            if (ejecutar == enumEjecutar.Parte2)
            {
                button3_Click(null, new EventArgs());
            }
            if (ejecutar == enumEjecutar.Parte3)
            {
                bGeneraGV_Click(null, new EventArgs());
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            archFolders = new StreamReader(ConfigurationManager.AppSettings["FoldersToParse"]);

            var nombre_arch = string.Format("Revision-{0}.csv", DateTime.Now.ToString("yyyy-M-dd--HH-mm-ss"));
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            nombre_archC = string.Format("Revision-{0}-composite.csv", DateTime.Now.ToString("yyyy-M-dd--HH-mm-ss"));

            borraTodoBD();

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
                            service_name = parseComposite(linea + "\\composite.xml", linea, out port_type);
                            if (service_name != "")
                            {
                                grabarService(service_name, port_type, linea);

                                //parsear cada bpel
                                foreach (var bpel in bpels)
                                {
                                    parseBpel(linea + "\\" + bpel, linea);
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

            if (ejecutar != enumEjecutar.No)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Terminado!");
            }
        }

        private void lineaArchivoC(string linea)
        {
            archC = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_archC, true);
            archC.WriteLine(linea);
            archC.Close();
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
                                        docJca.Load(folder_name + "\\" + oJca.config);
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
                        RevisaNodo(nodeStep, fio.Name, folderName);
                    }
                }
            }
            else
            {
                arch.WriteLine("ERROR: ARCHIVO NO EXISTE!!  : {0}", file_name);
            }
        }

        private void RevisaNodo(XmlNode nodo, string file_name, string folderName)
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
                            operation_id = grabaOperacion(operation_name, service_name, file_name, folderName, my_port_type);
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
                            operation_id = grabaOperacion(operation_name, service_name, file_name, folderName, my_port_type);
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
                            operation_id = grabaOperacion(operation_name, service_name, file_name, folderName, my_port_type);

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
                                grabaRelacionQueue(operation_id, operation_name, service_name, my_port_type, operation_invoked, partner_link, port_type, folderName, file_name);
                            }
                        }
                    }
                    if (nodo.HasChildNodes)
                    {
                        foreach (XmlNode nodoHijo in nodo.ChildNodes)
                        {
                            RevisaNodo(nodoHijo, file_name, folderName);
                        }
                    }

                    if (nodo.Name == "invoke" && operation_name == "")
                    {
                        // casos donde el invoke va en fauls antes de obtener el nombre de la operacion
                        XmlDocument doc2 = new XmlDocument();
                        doc2.Load(folderName + "\\" + file_name);

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

                        grabaRelacion(operation_id, operation_name, service_name, my_port_type, operation_invoked, partner_link, port_type, folderName, file_name);
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
            var ctf = Path.ChangeExtension(folderName + "\\" + archivo, ".componentType");
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
                    wsdl = folderName + "\\" + plref.wsdlLocation;
                    wsdl2 = plref.wsdlLocation;

                    jca = folderName + "\\" + plref.config;
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
                                wsdl = folderName + "\\" + nodeStep.Attributes["ui:wsdlLocation"].Value;
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
                        jca = folderName + "\\" + ljca.config;
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
                                jca = folderName + "\\" + xBind.Value;
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

        private long grabaOperacion(string operationName, string serviceName, string bpel, string folderName, string portType)
        {
            long id_ret = 0;
            using (var cmd = new MySqlCommand())
            {
                portType = fixServiceName(service_name, portType, folderName);
                cmd.Connection = conn;
                cmd.CommandText = "insert into operations (name, service_name, bpel, folder_name, port_type, fecha, real_name) values (@operation, @service, @bpel, @folder, @port_type, now(), @real_name);    SELECT  last_insert_id() ";
                cmd.CommandType = CommandType.Text;

                //cmd.Parameters.Add(new OracleParameter("operation", fixOperationName(operationName, service)));
                cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                cmd.Parameters.Add(new MySqlParameter("bpel", bpel));
                cmd.Parameters.Add(new MySqlParameter("folder", folderName));
                portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpel, folderName, portType)));

                try
                {
                    id_ret = Convert.ToInt64(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    var m = ex.Message;
                    id_ret = 0;
                }
            }

            if (operation_id == 0 && id_ret != 0)
            {
                //actualizar relaciones creadas antes del bpel
                using (var cmd = new MySqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "update operations_rel set operation_id = @operation_id where operation_id = 0 ";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(new MySqlParameter("operation_id", id_ret));

                    cmd.ExecuteNonQuery();
                }
            }
            return id_ret;
        }

        private void grabaRelacion(long operation_id, string operationName, string serviceName, string portType, string operationInvoked, string serviceIvoked, string portTypeInv, string folderName, string bpelName)
        {
            try
            {
                if (operationName == null)
                {
                    return;
                }
                portType = fixServiceName(serviceName, portType, folderName);

                var cont = false;
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select count(*) from operations_rel where operation_name = @operation and service_name = @service and " +
                    "operation_invoked = @operation_invoked and service_invoked = @service_invoked and invoked_port_type = @invoked_port_type and real_name = @real_name and " +
                    "operation_id = @operation_id";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                    cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                    cmd.Parameters.Add(new MySqlParameter("operation_invoked", operationInvoked));
                    cmd.Parameters.Add(new MySqlParameter("service_invoked", serviceIvoked));
                    portTypeInv = portTypeInv.Contains(":") ? portTypeInv.Remove(0, portTypeInv.IndexOf(":") + 1) : portTypeInv;
                    cmd.Parameters.Add(new MySqlParameter("invoked_port_type", fixPortType(operationName, serviceName, "", folderName, portType)));
                    portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                    cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                    cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpelName, folderName, portType)));
                    cmd.Parameters.Add(new MySqlParameter("operation_id", operation_id));

                    var respS = cmd.ExecuteScalar();
                    cont = (Convert.ToInt32(respS) == 0);
                }
                if (cont)
                {
                    portTypeInv = fixServiceNameInv(serviceIvoked, portTypeInv);

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "insert into operations_rel (operation_name, service_name, operation_invoked, service_invoked, fecha, invoked_port_type, port_type, real_name, operation_id) values " +
                        "(@operation, @service, @operation_invoked, @service_invoked, now(), @invoked_port_type, @port_type, @real_name, @operation_id ) ";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                        cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                        portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                        cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                        cmd.Parameters.Add(new MySqlParameter("operation_invoked", operationInvoked));
                        cmd.Parameters.Add(new MySqlParameter("service_invoked", serviceIvoked));
                        portTypeInv = portTypeInv.Contains(":") ? portTypeInv.Remove(0, portTypeInv.IndexOf(":") + 1) : portTypeInv;
                        cmd.Parameters.Add(new MySqlParameter("invoked_port_type", portTypeInv));
                        cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpelName, folderName, portType)));
                        cmd.Parameters.Add(new MySqlParameter("operation_id", operation_id));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        private void grabaRelacionQueue(long operation_id, string operationName, string serviceName, string portType, string operationInvoked, string serviceIvoked, string portTypeInv, string folderName, string bpelName)
        {
            try
            {
                if (operationName == null)
                {
                    return;
                }
                portType = fixServiceName(serviceName, portType, folderName);

                var cont = false;
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select count(*) from operations_dequeue where operation_name = @operation and service_name = @service and " +
                    "queue_invoked = @queue_invoked and service_invoked = @service_invoked and queue_type = @queue_type and real_name = @real_name and " +
                    "operation_id = @operation_id";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                    cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                    cmd.Parameters.Add(new MySqlParameter("queue_invoked", operationInvoked));
                    cmd.Parameters.Add(new MySqlParameter("service_invoked", serviceIvoked));
                    portTypeInv = portTypeInv.Contains(":") ? portTypeInv.Remove(0, portTypeInv.IndexOf(":") + 1) : portTypeInv;
                    cmd.Parameters.Add(new MySqlParameter("queue_type", fixPortType(operationName, serviceName, "", folderName, portType)));
                    portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                    cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                    cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpelName, folderName, portType)));
                    cmd.Parameters.Add(new MySqlParameter("operation_id", operation_id));

                    var respS = cmd.ExecuteScalar();
                    cont = (Convert.ToInt32(respS) == 0);
                }
                if (cont)
                {
                    portTypeInv = fixServiceNameInv(serviceIvoked, portTypeInv);

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "insert into operations_dequeue (operation_name, service_name, queue_invoked, service_invoked, fecha, queue_type, port_type, real_name, operation_id) values " +
                        "(@operation, @service, @queue_invoked, @service_invoked, now(), @queue_type, @port_type, @real_name, @operation_id ) ";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add(new MySqlParameter("operation", operationName));
                        cmd.Parameters.Add(new MySqlParameter("service", serviceName));
                        portType = portType.Contains(":") ? portType.Remove(0, portType.IndexOf(":") + 1) : portType;
                        cmd.Parameters.Add(new MySqlParameter("port_type", portType));
                        cmd.Parameters.Add(new MySqlParameter("queue_invoked", operationInvoked));
                        cmd.Parameters.Add(new MySqlParameter("service_invoked", serviceIvoked));
                        portTypeInv = portTypeInv.Contains(":") ? portTypeInv.Remove(0, portTypeInv.IndexOf(":") + 1) : portTypeInv;
                        cmd.Parameters.Add(new MySqlParameter("queue_type", portTypeInv));
                        cmd.Parameters.Add(new MySqlParameter("real_name", fixPortType(operationName, serviceName, bpelName, folderName, portType)));
                        cmd.Parameters.Add(new MySqlParameter("operation_id", operation_id));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private void grabarService(string service_name, string port_type, string ruta)
        {
            port_type = fixServiceName(service_name, port_type, ruta);

            try
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "insert into services (name, fecha, port_type) values (@service_name, now(), @port_type) ";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(new MySqlParameter("service_name", service_name));
                    cmd.Parameters.Add(new MySqlParameter("port_type", port_type));

                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
        }

        private string fixPortType(string operationName, string serviceName, string bpel, string folderName, string portType)
        {
            var realName = portType + "." + operationName;
            if (portType == "Consume_Message_ptt" || portType == "PublishWOEventCS" || portType == "PublishEventCS" || portType == "IIntegrationMailbox" ||
                (folderName == @"C:\GIT\integration-dev-QA\5.SOA\DTVLA-Services\Vista360\Vista360-Services" && bpel == "GetVista360.bpel") ||
                (folderName == @"C:\GIT\integration-dev-QA\5.SOA\BBVOD\BBVODEventFromPEGS" && bpel == "IOrderableEventCatalogConfigurationService") ||
                (folderName == @"C:\GIT\integration-dev-QA\5.SOA\LeadershipInMovies\LeadershipInMovies") ||
                operationName.ToLower().Contains("consume") || operationName.ToLower().Contains("publish") ||
                operationName.ToLower().Equals("receive") || portType.ToLower().Contains("async") || portType == "EstablishCustomerRelationship_pt"
                )
            {
                realName = bpel + "#" + portType + "." + operationName;
            }
            return realName;

        }

        private string fixServiceName(string service_name, string port_type, string ruta)
        {
            if (ruta.Contains("ActivateServiceAR") || ruta.Contains("ActivateServiceCH") ||
                ruta.Contains("ActivateServiceCO") || ruta.Contains("ActivateServiceEC") ||
                ruta.Contains("ActivateServicePE") || ruta.Contains("ActivateServiceVE") ||
                ruta.Contains("ActiveServiceUY"))
            {

                return "ActivateService_pt-" + ruta.Substring(ruta.Length - 2);
            }
            else if (ruta.Contains("crm-assurance") && service_name.Contains("360"))
            {
                return "ptVista360-crm";
            }
            else if (ruta.Contains("IntArgFinance") && service_name.Contains("MedIBSFinanceService_ep"))
            {
                return "IBSFinanceServiceSoap";
            }
            else if (service_name.StartsWith("EDA"))
            {
                return service_name;
            }
            else
            {
                return port_type;
            }
        }

        private string fixServiceNameInv(string service_name, string port_type)
        {
            if (service_name.Contains("ActivateServiceAR") || service_name.Contains("ActivateServiceCH") ||
                service_name.Contains("ActivateServiceCO") || service_name.Contains("ActivateServiceEC") ||
                service_name.Contains("ActivateServicePE") || service_name.Contains("ActivateServiceVE") ||
                service_name.Contains("ActivateServiceUY") || service_name.Contains("ActivateServiceCL")
                )
            {

                return "ActivateService_pt-" + service_name.Substring(service_name.Length - 2);
            }
            else
            {
                return port_type;
            }
        }

        private void borraTodoBD()
        {
            try
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from operations_dequeue";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from operations_rel";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from operations";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from services";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //crear archivos
            Console.WriteLine("Exportando Asset a Crear");
            var nombre_arch = string.Format("Asset-A-Crear.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            using (var cmd = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", conn))
            {
                cmd.ExecuteNonQuery();
            }

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select * from operations o where o.ASSET_ID is null ";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("Nombre\tServicio\tBpel\tCarpeta\tPortType\tRealName");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", reader["name"].ToString(), reader["service_name"].ToString(), reader["bpel"].ToString(), reader["folder_name"].ToString(), reader["port_type"].ToString(), reader["real_name"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();


            Console.WriteLine("Exportando relacion en OER, no en GIT");
            nombre_arch = string.Format("En-OER-no-en-GIT.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select a2.id, a2.name, at.NAME as type_name, a2.real_name real_name_a2, a.id as id1, a.name as name1, " +
                    "   a.REAL_NAME as REAL_NAME1, at2.NAME as type_name2, ar.asset_id, ar.related_asset_id " +
                    "from ASSET_RELATION ar inner join asset a on ar.RELATED_ASSET_ID = a.id " +
                    "inner join asset a2 on ar.ASSET_ID = a2.id " +
                    "inner join asset_type at on a2.type = at.id  " +
                    "inner join asset_type at2 on  a.type = at2.id " +
                    "where  ar.RELATION_TYPE in ( 50000, 50001 ) and ar.RELATED_PS = 'P' and at.id in (145, 50004) and a2.name not like '%BUSDTV%' and " +
                    "a.id not in ( " +
                    "   select opr.INVOKED_ASSET_ID from operations_rel opr inner join operations o on opr.real_name = o.real_name where o.ASSET_ID = ar.ASSET_id ) ";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();

                arch.WriteLine("Id\tNombre\tRealName\tType\tId2\tName2\tRealName2\tType2\trelated_asset_id");

                //grabar resultado en archivo
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}", reader["id"].ToString(), reader["name"].ToString(),
                        reader["real_name_a2"].ToString(), reader["type_name"].ToString(), reader["id1"].ToString(),
                        reader["name1"].ToString(), reader["REAL_NAME1"].ToString(), reader["type_name"].ToString(),
                        reader["type_name2"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();



            Console.WriteLine("Exportando aplicaciones sin referencias a los servicios de las operaciones que invocan");
            nombre_arch = string.Format("APP-Sin-Referencias-A-Servicios.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select a.id, a.name, ad.id id_oper, ad.name oper_name, ad2.id id_service, ad2.name service_name " +
                    "from asset a " +
                    "inner join ASSET_RELATION ar on ar.ASSET_ID = a.id " +
                    "inner join asset ad on ad.id = ar.RELATED_ASSET_ID " +
                    "inner join ASSET_RELATION ar2 on ar2.ASSET_ID = ad.id " +
                    "inner join asset ad2 on ad2.id = ar2.RELATED_ASSET_ID " +
                    "where a.type in (158) and " +
                    "ar.RELATION_TYPE = 50000 and ar.RELATED_PS = 'P' and " +
                    "ar2.RELATION_TYPE = 108  and ar2.RELATED_PS = 'P' and not exists " +
                    "(select 1 " +
                    "from ASSET_RELATION ar3 " +
                    "where  ar3.ASSET_ID = a.id and ar3.RELATED_ASSET_ID = ad2.id and " +
                    "  ar3.RELATION_TYPE = 118 and ar3.RELATED_PS = 'S')" +
                    "order by 1;";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("AssetId\tNombre\tIdOper\tOperName\tIdService\tServiceInvoked");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", reader["id"].ToString(), reader["name"].ToString(),
                        reader["id_oper"].ToString(), reader["oper_name"].ToString(),
                        reader["id_service"].ToString(), reader["service_name"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();


            Console.WriteLine("Exportando aplicaciones con referencias a servicios y que no invoca a sus operaciones");
            nombre_arch = string.Format("APP-Con-Referencias-Sobrantes.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select a.id, a.name, ase.id id_service, ase.name service_name " +
                    "from asset a " +
                    "inner join ASSET_RELATION ar3 on ar3.ASSET_ID = a.id " +
                    "inner join asset ase on ase.id = ar3.RELATED_ASSET_ID " +
                    "where a.type in (158) and  ar3.RELATION_TYPE = 118 and ar3.RELATED_PS = 'S' " +
                    "and not exists " +
                    "(select 1 " +
                    "from ASSET_RELATION ar " +
                    "inner join ASSET_RELATION ar2 on ar2.ASSET_ID = ar.RELATED_ASSET_ID " +
                    "where ar.ASSET_ID = a.id and ar.RELATION_TYPE = 50000 and ar.RELATED_PS = 'P' and ar2.RELATION_TYPE = 108 and ar2.RELATED_PS = 'P' and  " +
                    " ar2.RELATED_ASSET_ID = ase.id)" +
                    "order by 1;";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("AssetId\tNombre\tIdService\tServiceInvoked");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}", reader["id"].ToString(), reader["name"].ToString(),
                        reader["id_service"].ToString(), reader["service_name"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();

            Console.WriteLine("Exportando relacion en GIT, no en OER");
            nombre_arch = string.Format("En-GIT-no-en-OER.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select o.ASSET_ID, opr.*,  o.REAL_NAME " +
                    "from operations_rel opr inner join operations o on opr.operation_id = o.operation_id " +
                    "where opr.INVOKED_ASSET_ID not in ( " +
                    "select ar.RELATED_ASSET_ID from ASSET_RELATION ar  " +
                    "where ar.ASSET_id =  o.ASSET_ID and ar.RELATION_TYPE in ( 50000, 50001 ) ) " +
                    "order by 2,1";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("AssetId\tNombre\tServiceName\tRealName\tOperationInvoked\tServiceInvoked\tInvokedAssetId\tInvokedPortType");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", reader["asset_id"].ToString(), reader["operation_name"].ToString(),
                        reader["service_name"].ToString(), reader["REAL_NAME"].ToString(),
                        reader["operation_invoked"].ToString(), reader["service_invoked"].ToString(), reader["INVOKED_ASSET_ID"].ToString(),
                        reader["invoked_port_type"].ToString(), reader["port_type"].ToString());
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();




            Console.WriteLine("Exportando relacion en GIT, no en OER de colas");
            nombre_arch = string.Format("En-GIT-no-en-OER-COLAS.txt");
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            //ejecutar sql
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT o.asset_id, od.*  " +
                    "from operations_dequeue od inner join operations o on o.operation_id = od.operation_id and not exists " +
                    "(select 1 from ASSET_RELATION ar " +
                    "inner join asset a on a.id = ar.ASSET_ID  " +
                    "where a.id = o.asset_id and ar.RELATION_TYPE in ( 50100) and ar.related_ps = 'P')  " +
                    "order by 2,1";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                arch.WriteLine("AssetId\tNombre\tServiceName\tRealName\tQueueInvoked\tQueueType\tInvokedAssetId");
                while (reader.Read())
                {
                    arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", reader["asset_id"].ToString(), reader["operation_name"].ToString(),
                        reader["service_name"].ToString(), reader["REAL_NAME"].ToString(),
                        reader["queue_invoked"].ToString(), reader["queue_type"].ToString(), reader["INVOKED_ASSET_ID"].ToString()
                        );
                }
                reader.Close();
                reader = null;
            }
            //cerrar archivo
            arch.Close();


            if (ejecutar != enumEjecutar.No)
            {
                this.Close();
            }
        }

        private void bGeneraGV_Click(object sender, EventArgs e)
        {
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

            var nombre_arch_cmd = ConfigurationManager.AppSettings["PathGV"] + "genera_todo.cmd";
            var arch_cmd = new StreamWriter(nombre_arch_cmd, false);

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
                    "inner join ASSET_RELATION ar on ar.ASSET_ID = a.id " +
                    "inner join asset ad on ad.id = ar.RELATED_ASSET_ID " +
                    "inner join asset a2 on  a2.id = ad.id " +
                    "inner join ASSET_RELATION ar2 on ar2.ASSET_ID = a2.id " +
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

            if (ejecutar != enumEjecutar.No)
            {
                this.Close();
            }
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
