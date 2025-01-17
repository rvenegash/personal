namespace apialmuerzos.Models
{
    public class DetallePedido
    {
        public DetalleMenu detallePedido { get; set; }
        public Pedido pedido { get; set; }
        public int cantidad { get; set; }
        public int total { get; set; }
    }
}
