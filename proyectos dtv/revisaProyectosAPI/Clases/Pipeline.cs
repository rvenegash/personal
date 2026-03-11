using System;
using System.Collections.Generic;
using System.Text;

namespace revisaProyectosAPI.Clases
{
    class Pipeline
    {
        public string name { get; set; }
        public string systemId { get; set; }
        public string route { get; set; }
        public string operation { get; set; }
        public string param_ServiceEndpoint { get; set; }
        public string param_operation { get; set; }
    }
}
