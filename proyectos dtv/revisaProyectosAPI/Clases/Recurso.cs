using System;
using System.Collections.Generic;
using System.Text;

namespace revisaProyectosAPI.Clases
{
    class Recurso
    {
        public string nombre { get; set; }
        public string metodo { get; set; }
        public List<Entidad> operaciones { get; set; }
    }
}
