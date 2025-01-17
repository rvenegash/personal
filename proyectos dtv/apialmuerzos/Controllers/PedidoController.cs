using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace apialmuerzos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        //[HttpGet]
        //public ActionResult<Models.Usuario> GetDetalleMenu(int codMenu, int codDetalleMenu, [FromServices] IConfiguration config)
        //{
        //    var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
        //    using (MySqlConnection conn = new MySqlConnection(mysqldb))
        //    {
        //        conn.Open();

        //        string sql = "SELECT D.*, O.nombre as nombreOpcion FROM DetalleMenu D INNER JOIN OpcionesMenu O on O.idOpcionesMenu = D.idOpcionesMenu WHERE D.idMenu = @ID AND D.idOpcionesMenu = @IDDET";
        //        using (var cmd = new MySqlCommand(sql, conn))
        //        {
        //            cmd.CommandType = CommandType.Text;
        //            cmd.Parameters.Add(new MySqlParameter("ID", codMenu));
        //            cmd.Parameters.Add(new MySqlParameter("IDDET", codDetalleMenu));

        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    var obj = new Models.DetalleMenu()
        //                    {
        //                        menu = new Models.Menu() { id = Rutinas.intNul(reader, "idMenu") },
        //                        opcion = new Models.OpcionesMenu() { id = Rutinas.intNul(reader, "idOpcionesMenu"), nombre = Rutinas.stringNul(reader, "nombreOpcion") },
        //                        precioReal = Rutinas.intNul(reader, "precioReal")
        //                    };

        //                    return Ok(obj);
        //                }
        //            }
        //        }
        //    }

        //    return NotFound();
        //}

        [HttpGet]
        [Route("usuarios")]
        public ActionResult<IEnumerable<Models.Pedido>> GetDetalleMenuDeMenu(DateTime fecha, [FromServices] IConfiguration config)
        {
            var list = new List<Models.Pedido>();

            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "SELECT P.*, U.nombre as nombreUsuario, (SELECT DM.precioReal * DP.cantidad FROM DetallePedido DP INNER JOIN DetalleMenu DM on DM.idMenu = DP.idMenu AND DM.idOpcionesMenu = DP.idOpcionesMenu WHERE DP.idUsuario = P.idUsuario AND DP.fechaPedido = P.fechaPedido) as totalPedido FROM Pedido P INNER JOIN Usuario U on U.idUsuario = P.idUsuario WHERE P.fechaPedido = @FECHA ";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new MySqlParameter("FECHA", fecha));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obj = new Models.Pedido()
                            {
                                fecha = Rutinas.datetimeNul(reader, "fechaPedido"),
                                usuario = new Models.Usuario() { id = Rutinas.stringNul(reader, "idUsuario"), nombre = Rutinas.stringNul(reader, "nombreUsuario") },
                                pagado = Rutinas.stringNul(reader, "pagado"),
                                totalPedido = Rutinas.intNul(reader, "totalPedido")
                            };

                            list.Add(obj);
                        }
                    }
                }
            }

            return Ok(list);
        }

        [HttpPost]
        [Route("pago")]
        public ActionResult Post([FromServices] IConfiguration config, [FromBody] Models.Pedido value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "UPDATE Pedido SET pagado = 'S' WHERE idUsuario = @USR AND fechaPedido = @FECHA";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("USR", value.usuario.id));
                        cmd.Parameters.Add(new MySqlParameter("FECHA", value.fecha));

                        var cant = cmd.ExecuteNonQuery();
                        if (cant == 1)
                        {
                            return Ok();
                        }
                    }
                }
                return BadRequest();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

    }
}