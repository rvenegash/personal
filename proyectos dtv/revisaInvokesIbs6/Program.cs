using System;
using ScanDirectory;
using System.Xml;
using System.IO;
using revisaInvokesIbs6.Clases;
//using ScanDirectory;

namespace revisaInvokesIbs6
{
    class Program
    {
        static int level = 0;
        static StreamWriter aFileXml;

        static void Main1(string[] args)
        {
            String sFileXml = "C:\\Trabajo\\ServiciosUsadosRESB.csv";
            aFileXml = new StreamWriter(sFileXml, false);
            GrabaCabecera();

            ScanDirectory.ScanDirectory scanDirectory = new ScanDirectory.ScanDirectory();
            scanDirectory.FileEvent += new ScanDirectory.ScanDirectory.FileEventHandler(scanDirectory_FileEvent);
            scanDirectory.SearchPattern = "*.bpel";
            scanDirectory.WalkDirectory("C:\\JDeveloper\\mywork\\RESB");

            aFileXml.Close();

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.ReadKey();
        }

        private static void GrabaCabecera()
        {
            aFileXml.WriteLine("Folder,File,Invoke Name,Operation,PartnerLink,PortType");
        }

        private static void GrabaInvoke(FileInfo file, Invoke invoke)
        {
            String text = String.Format("{0},{1},{2},{3},{4},{5}", file.Directory.Name, file.Name, invoke.name, invoke.operation, invoke.partnerLink, invoke.portType);

            aFileXml.WriteLine(text);
        }

        private static void scanDirectory_FileEvent(object sender, FileEventArgs e)
        {
            String file_name = e.Info.FullName;

            Console.WriteLine(file_name);

            XmlDocument doc = new XmlDocument();
            doc.Load(file_name);

            Console.WriteLine("process");
            level = 0;

            XmlNodeList bdProcess = doc.GetElementsByTagName("process");
            foreach (XmlNode nodeP in bdProcess)
            {
                XmlElement subsElement = (XmlElement)nodeP;

                level += 1;
                Console.WriteLine("sequence");
                XmlNodeList bdSequence = subsElement.GetElementsByTagName("sequence");
                foreach (XmlNode nodeS in bdSequence)
                {
                    XmlElement subsSequence = (XmlElement)nodeS;

                    level += 1;
                    XmlNodeList steps = nodeS.ChildNodes;
                    foreach (XmlNode nodeStep in steps)
                    {
                        RevisaNodo(e.Info, nodeStep);
                    }
                    level -= 1;
                }
            }
        }

        private static void RevisaNodo(FileInfo file, XmlNode nodo)
        {
            String text = "";
            String name = "";
            if (nodo.Attributes != null)
            {
                try
                {
                    name = nodo.Attributes["name"].InnerText;
                }
                catch (Exception)
                {
                }
            }

            if (nodo.Name == "invoke")  //solo veo los invokes
            {
                Invoke invoke = InvokeFactory.CreateInvoke(nodo);

                text = String.Format("{0} Tipo {1}, nombre {2}, O {3}", Indent(level), nodo.Name, invoke.name, invoke.operation);
                Console.WriteLine(text);

                GrabaInvoke(file, invoke);

                if (nodo.HasChildNodes)
                {
                    level += 1;
                    foreach (XmlNode nodoHijo in nodo.ChildNodes)
                    {
                        RevisaNodo(file, nodoHijo);
                    }
                    level -= 1;
                }
            }
        }

        public static string Indent(int count)
        {
            return "".PadLeft(count * 2);
        }
    }
}
