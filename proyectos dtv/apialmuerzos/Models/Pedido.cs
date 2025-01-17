using System;

namespace apialmuerzos.Models
{
    public class Pedido
    {
        public DateTime fecha { get; set; }
        public Usuario usuario { get; set; }
        public string pagado { get; set; }
        public int totalPedido { get; set; }
    }
}
