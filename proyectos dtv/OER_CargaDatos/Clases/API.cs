using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OER_CargaDatos.Clases
{
    public class API
    {
        public string Api { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    public class ListaAPI
    {
        public List<API> Lista { get; set; }

    }
}
