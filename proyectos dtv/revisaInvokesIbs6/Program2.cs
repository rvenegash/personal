using System;
using System.IO;
using System.Xml;
using revisaInvokesIbs6.Clases;
using ScanDirectory;

namespace revisaInvokesIbs6
{
    class Program2
    {
        static int level = 0;
        static StreamWriter aFileXml;

        static void Main2(string[] args)
        {

            ScanDirectory.ScanDirectory scanDirectory = new ScanDirectory.ScanDirectory();
            scanDirectory.FileEvent += new ScanDirectory.ScanDirectory.FileEventHandler(scanDirectory_FileEvent);
            scanDirectory.SearchPattern = "*.bpel";
            scanDirectory.WalkDirectory("C:\\JDeveloper\\mywork\\RESB\\CRMSupportAndReadiness");

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.ReadKey();
        }

        private static void scanDirectory_FileEvent(object sender, FileEventArgs e)
        {
            String sFileXml = "C:\\Trabajo\\new_doc\\" + e.Info.Name + ".txt";
            aFileXml = new StreamWriter(sFileXml, false);
            GrabaCabecera();

            String file_name = e.Info.FullName;

            Console.WriteLine(file_name);

            XmlDocument doc = new XmlDocument();
            doc.Load(file_name);

            Console.WriteLine("process");
            level = 0;

            XmlNodeList bdProcess = doc.GetElementsByTagName("process");
            foreach (XmlNode nodeP in bdProcess)
            {

                level += 1;
                XmlElement subsElement = (XmlElement)nodeP;
                XmlNodeList stepsSE = subsElement.ChildNodes;
                foreach (XmlNode nodeStep in stepsSE)
                {
                    level += 1;
                    RevisaNodo(e.Info, nodeStep);
                    level -= 1;
                }

                //level += 1;
                //Console.WriteLine("sequence");
                //XmlNodeList bdSequence = subsElement.GetElementsByTagName("sequence");
                //foreach (XmlNode nodeS in bdSequence)
                //{
                //    XmlElement subsSequence = (XmlElement)nodeS;

                //    level += 1;
                //    XmlNodeList steps = subsSequence.ChildNodes;
                //    foreach (XmlNode nodeStep in steps)
                //    {
                //        RevisaNodo(e.Info, nodeStep);
                //    }
                //    level -= 1;
                //}
            }
            aFileXml.Close();

        }

        private static void RevisaNodo(FileInfo file, XmlNode nodo)
        {
            String text = "";

            switch (nodo.Name)
            {
                case "sequence":
                case "while":
                case "switch":
                case "case":
                case "otherwise":
                case "scope":
                case "invoke":
                case "reply":
                case "receive":
                case "assign":
                case "empty":
                case "faultHandlers":
                case "catch":
                    Invoke invoke = InvokeFactory.CreateInvoke(nodo);

                    text = String.Format("{0} Tipo {1}, nombre {2}, O {3}", Indent(level), nodo.Name, invoke.name, invoke.operation);
                    Console.WriteLine(text);

                    GrabaInicioInvoke(file, invoke, level);

                    if (nodo.HasChildNodes)
                    {
                        level += 1;
                        foreach (XmlNode nodoHijo in nodo.ChildNodes)
                        {
                            RevisaNodo(file, nodoHijo);
                        }
                        level -= 1;
                    }
                    GrabaFinInvoke(file, invoke, level);

                    break;
                default:
                    break;
            }
        }

        private static void GrabaCabecera()
        {
            aFileXml.WriteLine("Folder,File,Invoke Name,Operation,PartnerLink,PortType");
        }

        private static void GrabaInicioInvoke(FileInfo file, Invoke invoke, int Level)
        {
            String text = "";
            switch (invoke.innerName)
            {
                case "faultHandlers":
                case "empty":
                case "sequence":
                case "otherwise":
                case "reply":
                    text = String.Format("    {0}{1}", Indent(level), invoke.innerName);
                    break;
                case "switch":
                case "assign":
                    text = String.Format("    {0}{1} ({2})", Indent(level), invoke.innerName, invoke.name);
                    break;
                case "catch":
                    text = String.Format("    {0}{1} ({2})", Indent(level), invoke.innerName, invoke.faultVariable);
                    break;
                case "case":
                case "while":
                    text = String.Format("    {0}{1} ({2})", Indent(level), invoke.innerName, invoke.condition);
                    break;
                default:
                    text = String.Format("Ini {0}{1} ({2}) {3},{4},{5}", Indent(level), invoke.innerName, invoke.name, invoke.operation, invoke.partnerLink, invoke.portType);
                    break;
            }

            aFileXml.WriteLine(text);
        }

        private static void GrabaFinInvoke(FileInfo file, Invoke invoke, int Level)
        {
            String text = "";
            switch (invoke.innerName)
            {
                case "case":
                case "while":
                case "empty":
                case "reply":
                case "assign":
                case "invoke":
                    break;
                case "sequence":
                case "switch":
                case "catch":
                case "otherwise":
                    text = String.Format("Fin {0}{1}", Indent(level), invoke.innerName);
                    break;
                default:
                    text = String.Format("Fin {0}{1} ({2})", Indent(level), invoke.innerName, invoke.name);
                    break;
            }
            if (text != "")
                aFileXml.WriteLine(text);
        }

        public static string Indent(int count)
        {
            return "".PadLeft(count * 2);
        }
    }
}
