using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace revisaProyectosSCA.Clases
{
    public class GenericActivity : Activity
    {
        public string partnerLink { get; set; }
        public string portType { get; set; }
        public string name { get; set; }
        public string operation { get; set; }
        public string innerName { get; set; }
        public string condition { get; set; }
        public string target { get; set; }
        public string expression { get; set; }
        public string inputVariable { get; set; }
        public string outputVariable { get; set; }
        public string faultVariable { get; set; }
        public string variable { get; set; }
        public string bpelx_inputHeaderVariable { get; set; }
        public string indexVariable { get; set; }
    }
}
