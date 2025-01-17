using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace revisaProyectosSCA.Clases
{
   public class Servicio
   {
       public string Id { get; set; }
       public string Name { get; set; }
       public string Version { get; set; }

       public List<Operacion> Operaciones { get; set; }
    }
}
