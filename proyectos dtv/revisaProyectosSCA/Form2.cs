using System;
using System.Windows.Forms;
using System.Configuration;
using ScanDirectory;
using System.Xml;
using revisaProyectosSCA.Clases;
using System.Collections.Generic;
using System.IO;

namespace revisaProyectosSCA
{
    public partial class Form2 : Form
    {
        List<TreeNode> lVariables;
        List<string> keywords = new List<string> { "from", "bpelx:from", "bpelx:to", "to", "receive", "reply", "bpelx:target", "bpelx:validate", "invoke", "case", "bpelx:flowN", "throw", "catch", "bpelx:exec", "flow", "pick" };

        int level = 0;
        string operation_name;
        string service_name;
        StreamWriter arch;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbFolder.Text = ConfigurationManager.AppSettings["baseFolder"];
        }

        private void bOpen_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.SelectedPath.Equals(""))
                folderBrowserDialog1.SelectedPath = ConfigurationManager.AppSettings["baseFolder"];
            else
                folderBrowserDialog1.SelectedPath = tbFolder.Text;

            folderBrowserDialog1.ShowNewFolderButton = false;
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath.Length > 0)
            {
                tbFolder.Text = folderBrowserDialog1.SelectedPath;

                var x = new System.IO.DirectoryInfo(tbFolder.Text);
                service_name = x.Name;

                leeDirectorio(tbFolder.Text);
            }
        }

        private void leeDirectorio(string Path)
        {
            lbArtifact.Items.Clear();

            ScanDirectory.ScanDirectory scanDirectory = new ScanDirectory.ScanDirectory();
            scanDirectory.FileEvent += new ScanDirectory.ScanDirectory.FileEventHandler(scanDirectory_FileEvent);
            scanDirectory.SearchPattern = ConfigurationManager.AppSettings["folderPattern"];
            scanDirectory.WalkDirectory(Path);
        }

        private void scanDirectory_FileEvent(object sender, FileEventArgs e)
        {
            lbArtifact.Items.Add(e.Info.Name);
        }

        private void lbArtifact_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbArtifact.SelectedIndex >= 0)
            {
                lblArtifact.Text = (string)lbArtifact.SelectedItem;
                lblArtifact.Tag = tbFolder.Text + "\\" + lblArtifact.Text;
                tvParts.Nodes.Clear();
            }
        }

        private void bParseAll_Click(object sender, EventArgs e)
        {
            var nombre_arch = string.Format("{0}.csv", service_name);
            arch = new StreamWriter(ConfigurationManager.AppSettings["Path"] + nombre_arch, false);

            for (int i = 0; i < lbArtifact.Items.Count; i++)
            {
                operation_name = "";
                lbArtifact.SelectedIndex = i;

                bParse_Click(null, null);
            }

            arch.Close();
        }

        private void bParse_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            String file_name = lblArtifact.Tag.ToString();

            var fio = new System.IO.FileInfo(file_name);

            Console.WriteLine(file_name);

            XmlDocument doc = new XmlDocument();
            doc.Load(file_name);

            progressBar1.Minimum = 0;
            progressBar1.Value = 0;

            lVariables = new List<TreeNode>();
            level = 0;

            XmlNodeList bdProcess = doc.GetElementsByTagName("process");
            foreach (XmlNode nodeP in bdProcess)
            {

                XmlElement subsElement = (XmlElement)nodeP;
                XmlNodeList stepsSE = subsElement.ChildNodes;

                //TreeNode tn = tvParts.Nodes.Add(string.Format("<{0}> {1}", "process", nodeP.Attributes["name"].InnerText));
                //tvParts.SelectedNode = tn;

                progressBar1.Maximum = stepsSE.Count + 1;
                foreach (XmlNode nodeStep in stepsSE)
                {
                    progressBar1.Value += 1;
                    progressBar1.Update();

                    level += 1;
                    RevisaNodo(nodeStep, fio.Name);
                    level -= 1;
                }
                //check unused variables
                //checkUnusedVariables();
            }
            progressBar1.Visible = false;
        }

        //private void checkUnusedVariables()
        //{
        //    foreach (TreeNode item in lVariables)
        //    {
        //        foreach (TreeNode item2 in item.Nodes)
        //        {
        //            if (item2.Nodes.Count == 0)
        //            {
        //                item2.ForeColor = System.Drawing.Color.DarkRed;
        //            }
        //        }
        //    }
        //}

        private void RevisaNodo(XmlNode nodo, string file_name)
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

                case "variables":
                case "sequence":
                case "partnerLink":
                case "partnerLinks":
                case "while":
                case "switch":
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

                    //GenericActivity activity = GenericFactory.CreateGenericActivity(nodo);
                    //TreeNode tn = tvParts.SelectedNode.Nodes.Add(NombreNodo(activity));
                    //tvParts.SelectedNode = tn;
                    //if (nodo.Name == "variables")
                    //    lVariables.Add(tn);

                    if (nodo.Name == "receive" || nodo.Name == "onMessage")
                    {
                        operation_name = nodo.Attributes["operation"].InnerText;
                    }
                    if (nodo.HasChildNodes)
                    {
                        level += 1;
                        foreach (XmlNode nodoHijo in nodo.ChildNodes)
                        {
                            RevisaNodo(nodoHijo, file_name);
                        }
                        level -= 1;
                    }
                    //ActividadUsaVariable2(activity);

                    //tn = tvParts.SelectedNode.Parent;
                    //"variable"
                    if (nodo.Name == "invoke" && operation_name != "")
                    {
                        var operation_invoked = nodo.Attributes["operation"].InnerText;
                        var partner_link = nodo.Attributes["partnerLink"].InnerText;
                        var port_type = nodo.Attributes["portType"].InnerText;

                        arch.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", file_name, service_name, operation_name, operation_invoked, partner_link, port_type);
                    }
                    //tvParts.SelectedNode = tn;

                    break;
                default:
                    break;
            }
        }

        //private void RevisaUsoVariable(XmlNode nodo, string p, TreeNode nodoVar)
        //{
        //    switch (nodo.Name)
        //    {
        //        case "from":
        //        case "bpelx:from":

        //        case "bpelx:to":
        //        case "to":
        //        case "receive":
        //        case "reply":
        //        case "bpelx:target":

        //        case "bpelx:validate":

        //        case "invoke":

        //        case "case":

        //        case "bpelx:flowN":

        //        case "catch":

        //        case "bpelx:exec":

        //        //case "variable":
        //        //case "empty":
        //        //case "variables":
        //        //case "partnerLink":
        //        //case "partnerLinks":

        //        case "process":
        //        case "sequence":
        //        case "copy":
        //        case "assign":
        //        case "switch":
        //        case "otherwise":
        //        case "faultHandlers":
        //        case "bpelx:remove":
        //        case "bpelx:append":
        //        case "scope":
        //        case "catchAll":
        //        case "while":

        //            GenericActivity activity = GenericFactory.CreateGenericActivity(nodo);

        //            if (ActividadUsaVariable(nodo.Name, p, activity))
        //            {
        //                nodoVar.Nodes.Add(NombreNodo(activity));
        //            }
        //            if (nodo.HasChildNodes)
        //            {
        //                foreach (XmlNode nodoHijo in nodo.ChildNodes)
        //                {
        //                    RevisaUsoVariable(nodoHijo, p, nodoVar);
        //                }
        //            }

        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private bool ActividadUsaVariable(string NombreActividad, string NombreVar, GenericActivity activity)
        //{
        //    bool ret = false;
        //    switch (NombreActividad)
        //    {
        //        case "from":            //expression or variable
        //        case "bpelx:from":
        //            if (activity.variable != null)
        //            {
        //                ret = (activity.variable == NombreVar);
        //            }
        //            break;
        //        case "bpelx:to":        //variable
        //        case "to":
        //        case "receive":
        //        case "reply":
        //        case "bpelx:target":
        //            ret = (activity.variable == NombreVar);

        //            break;
        //        case "bpelx:validate":  //variables

        //            break;
        //        case "invoke":          // inputVariable y outputVariable

        //            ret = (activity.inputVariable == NombreVar || activity.outputVariable == NombreVar);
        //            break;
        //        case "case":            //condition

        //            break;
        //        case "bpelx:flowN":     // N y indexVariable
        //            ret = (activity.indexVariable == NombreVar);
        //            break;
        //        case "throw":           //faultVariable
        //        case "catch":
        //            ret = (activity.faultVariable == NombreVar);
        //            break;
        //        case "bpelx:exec":      //<![CDATA[

        //            break;
        //        default:
        //            break;
        //    }
        //    return ret;
        //}

        //private void ActividadUsaVariable2(GenericActivity activity)
        //{
        //    if (keywords.Contains(activity.innerName))
        //    {
        //        foreach (TreeNode item in lVariables)
        //        {
        //            foreach (TreeNode item2 in item.Nodes)
        //            {
        //                string NombreVar = item2.Text;
        //                bool ret = false;
        //                switch (activity.innerName)
        //                {
        //                    case "from":            //expression or variable
        //                    case "bpelx:from":
        //                        if (activity.variable != null)
        //                        {
        //                            ret = (activity.variable == NombreVar);
        //                        }
        //                        break;
        //                    case "bpelx:to":        //variable
        //                    case "to":
        //                    case "receive":
        //                    case "reply":
        //                    case "bpelx:target":
        //                        ret = (activity.variable == NombreVar);

        //                        break;
        //                    case "bpelx:validate":  //variables

        //                        break;
        //                    case "invoke":          // inputVariable y outputVariable

        //                        ret = (activity.inputVariable == NombreVar || activity.outputVariable == NombreVar);
        //                        break;
        //                    case "case":            //condition

        //                        break;
        //                    case "bpelx:flowN":     // N y indexVariable
        //                        ret = (activity.indexVariable == NombreVar);
        //                        break;
        //                    case "throw":           //faultVariable
        //                    case "catch":
        //                        ret = (activity.faultVariable == NombreVar);
        //                        break;
        //                    case "bpelx:exec":      //<![CDATA[

        //                        break;
        //                    default:
        //                        break;
        //                }
        //                if (ret)
        //                {
        //                    item2.Nodes.Add(NombreNodo(activity));
        //                }
        //            }
        //        }
        //    }
        //}

        //string NombreNodo(GenericActivity nodo)
        //{
        //    string text = "";

        //    switch (nodo.innerName)
        //    {
        //        case "bpelx:from":
        //        case "from":
        //            text = string.Format("<{0}> {1} {2}", nodo.innerName, nodo.variable, nodo.expression);
        //            break;
        //        case "bpelx:to":
        //        case "to":
        //            text = string.Format("<{0}> {1}", nodo.innerName, nodo.variable);
        //            break;

        //        case "case":
        //            text = string.Format("<{0}> {1}", nodo.innerName, nodo.condition);
        //            break;

        //        case "variable":
        //            text = string.Format("{0}", nodo.name);
        //            break;
        //        case "bpelx:validate":
        //        case "partnerLinks":
        //        case "variables":
        //        case "while":
        //        case "switch":
        //        case "otherwise":
        //        case "scope":
        //        case "invoke":
        //        case "reply":
        //        case "partnerLink":
        //        case "receive":
        //        case "assign":
        //        case "empty":
        //        case "faultHandlers":
        //        case "catch":
        //        case "bpelx:exec":
        //        case "bpelx:append":
        //        case "bpelx:remove":
        //        case "bpelx:target":
        //        case "bpelx:flowN":
        //        case "flow":
        //            text = string.Format("<{0}> {1}", nodo.innerName, nodo.name);
        //            break;
        //        case "copy":
        //        case "sequence":
        //        case "catchAll":
        //            text = string.Format("<{0}>", nodo.innerName);
        //            break;
        //        default:
        //            text = string.Format("<{0}>", nodo.innerName);
        //            break;
        //    }
        //    return text;
        //}

    }
}
