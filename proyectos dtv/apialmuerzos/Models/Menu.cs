using System;

namespace apialmuerzos.Models
{
    public class Menu
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string activo { get; set; }
        public string cerrado { get; set; }
    }
}
