using System;
using System.Windows.Forms;
using System.Configuration;
using ScanDirectory;
using System.Xml;
using revisaProyectosSCA.Clases;
using System.Collections.Generic;

namespace revisaProyectosSCA
{
    public partial class Form1 : Form
    {
        List<TreeNode> lVariables;
        List<string> keywords = new List<string> { "from", "bpelx:from", "bpelx:to", "to", "receive", "reply", "bpelx:target", "bpelx:validate", "invoke", "case", "bpelx:flowN", "throw", "catch", "bpelx:exec", "flow" };

        int level = 0;
        public Form1()
        {
            InitializeComponent();
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

        private void bParse_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            String file_name = lblArtifact.Tag.ToString();

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

                TreeNode tn = tvParts.Nodes.Add(string.Format("<{0}> {1}", "process", nodeP.Attributes["name"].InnerText));
                tvParts.SelectedNode = tn;

                progressBar1.Maximum = stepsSE.Count + 1;
                foreach (XmlNode nodeStep in stepsSE)
                {
                    progressBar1.Value += 1;
                    progressBar1.Update();

                    level += 1;
                    RevisaNodo(nodeStep);
                    level -= 1;
                }
                //check unused variables
                checkUnusedVariables();
            }
            progressBar1.Visible = false;
        }

        private void checkUnusedVariables()
        {
            foreach (TreeNode item in lVariables)
            {
                foreach (TreeNode item2 in item.Nodes)
                {
                    if (item2.Nodes.Count == 0)
                    {
                        item2.ForeColor = System.Drawing.Color.DarkRed;
                    }
                }
            }
        }

        private void RevisaNodo(XmlNode nodo)
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

                    GenericActivity activity = GenericFactory.CreateGenericActivity(nodo);
                    TreeNode tn = tvParts.SelectedNode.Nodes.Add(NombreNodo(activity));
                    tvParts.SelectedNode = tn;
                    if (nodo.Name == "variables")
                        lVariables.Add(tn);

                    if (nodo.HasChildNodes)
                    {
                        level += 1;
                        foreach (XmlNode nodoHijo in nodo.ChildNodes)
                        {
                            RevisaNodo(nodoHijo);
                        }
                        level -= 1;
                    }
                    ActividadUsaVariable2(activity);

                    tn = tvParts.SelectedNode.Parent;
                    //"variable"
                    //if (nodo.Name == "variable")
                    //{
                    //    RevisaUsoVariable(nodo.ParentNode.ParentNode, activity.name, tvParts.SelectedNode);
                    //}
                    tvParts.SelectedNode = tn;

                    break;
                default:
                    break;
            }
        }

        private void RevisaUsoVariable(XmlNode nodo, string p, TreeNode nodoVar)
        {
            switch (nodo.Name)
            {
                case "from":
                case "bpelx:from":

                case "bpelx:to":
                case "to":
                case "receive":
                case "reply":
                case "bpelx:target":

                case "bpelx:validate":

                case "invoke":

                case "case":

                case "bpelx:flowN":

                case "catch":

                case "bpelx:exec":

                //case "variable":
                //case "empty":
                //case "variables":
                //case "partnerLink":
                //case "partnerLinks":

                case "process":
                case "sequence":
                case "copy":
                case "assign":
                case "switch":
                case "otherwise":
                case "faultHandlers":
                case "bpelx:remove":
                case "bpelx:append":
                case "scope":
                case "catchAll":
                case "while":

                    GenericActivity activity = GenericFactory.CreateGenericActivity(nodo);

                    if (ActividadUsaVariable(nodo.Name, p, activity))
                    {
                        nodoVar.Nodes.Add(NombreNodo(activity));
                    }
                    if (nodo.HasChildNodes)
                    {
                        foreach (XmlNode nodoHijo in nodo.ChildNodes)
                        {
                            RevisaUsoVariable(nodoHijo, p, nodoVar);
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private bool ActividadUsaVariable(string NombreActividad, string NombreVar, GenericActivity activity)
        {
            bool ret = false;
            switch (NombreActividad)
            {
                case "from":            //expression or variable
                case "bpelx:from":
                    if (activity.variable != null)
                    {
                        ret = (activity.variable == NombreVar);
                    }
                    break;
                case "bpelx:to":        //variable
                case "to":
                case "receive":
                case "reply":
                case "bpelx:target":
                    ret = (activity.variable == NombreVar);

                    break;
                case "bpelx:validate":  //variables

                    break;
                case "invoke":          // inputVariable y outputVariable

                    ret = (activity.inputVariable == NombreVar || activity.outputVariable == NombreVar);
                    break;
                case "case":            //condition

                    break;
                case "bpelx:flowN":     // N y indexVariable
                    ret = (activity.indexVariable == NombreVar);
                    break;
                case "throw":           //faultVariable
                case "catch":
                    ret = (activity.faultVariable == NombreVar);
                    break;
                case "bpelx:exec":      //<![CDATA[

                    break;
                default:
                    break;
            }
            return ret;
        }

        private void ActividadUsaVariable2(GenericActivity activity)
        {
            if (keywords.Contains(activity.innerName))
            {
                foreach (TreeNode item in lVariables)
                {
                    foreach (TreeNode item2 in item.Nodes)
                    {
                        string NombreVar = item2.Text;
                        bool ret = false;
                        switch (activity.innerName)
                        {
                            case "from":            //expression or variable
                            case "bpelx:from":
                                if (activity.variable != null)
                                {
                                    ret = (activity.variable == NombreVar);
                                }
                                break;
                            case "bpelx:to":        //variable
                            case "to":
                            case "receive":
                            case "reply":
                            case "bpelx:target":
                                ret = (activity.variable == NombreVar);

                                break;
                            case "bpelx:validate":  //variables

                                break;
                            case "invoke":          // inputVariable y outputVariable

                                ret = (activity.inputVariable == NombreVar || activity.outputVariable == NombreVar);
                                break;
                            case "case":            //condition

                                break;
                            case "bpelx:flowN":     // N y indexVariable
                                ret = (activity.indexVariable == NombreVar);
                                break;
                            case "throw":           //faultVariable
                            case "catch":
                                ret = (activity.faultVariable == NombreVar);
                                break;
                            case "bpelx:exec":      //<![CDATA[

                                break;
                            default:
                                break;
                        }
                        if (ret)
                        {
                            item2.Nodes.Add(NombreNodo(activity));
                        }
                    }
                }
            }
        }

        string NombreNodo(GenericActivity nodo)
        {
            string text = "";

            switch (nodo.innerName)
            {
                case "bpelx:from":
                case "from":
                    text = string.Format("<{0}> {1} {2}", nodo.innerName, nodo.variable, nodo.expression);
                    break;
                case "bpelx:to":
                case "to":
                    text = string.Format("<{0}> {1}", nodo.innerName, nodo.variable);
                    break;

                case "case":
                    text = string.Format("<{0}> {1}", nodo.innerName, nodo.condition);
                    break;

                case "variable":
                    text = string.Format("{0}", nodo.name);
                    break;
                case "bpelx:validate":
                case "partnerLinks":
                case "variables":
                case "while":
                case "switch":
                case "otherwise":
                case "scope":
                case "invoke":
                case "reply":
                case "partnerLink":
                case "receive":
                case "assign":
                case "empty":
                case "faultHandlers":
                case "catch":
                case "bpelx:exec":
                case "bpelx:append":
                case "bpelx:remove":
                case "bpelx:target":
                case "bpelx:flowN":
                case "flow":
                    text = string.Format("<{0}> {1}", nodo.innerName, nodo.name);
                    break;
                case "copy":
                case "sequence":
                case "catchAll":
                    text = string.Format("<{0}>", nodo.innerName);
                    break;
                default:
                    text = string.Format("<{0}>", nodo.innerName);
                    break;
            }
            return text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbFolder.Text = ConfigurationManager.AppSettings["baseFolder"];
        }

    }
}
