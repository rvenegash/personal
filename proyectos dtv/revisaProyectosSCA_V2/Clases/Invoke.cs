using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace revisaProyectosSCA_V2.Clases
{
    public class InvokeActivity : Activity
    {
        public string partnerLink { get; set; }
        public string portType { get; set; }
        public string name { get; set; }
        public string operation { get; set; }
        public string innerName { get; set; }
        public string condition { get; set; }
        public string faultVariable { get; set; }
    }
}
