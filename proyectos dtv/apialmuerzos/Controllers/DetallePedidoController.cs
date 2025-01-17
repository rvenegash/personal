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
    public class DetallePedidoController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Models.DetallePedido>> GetDetallePedidoDeUsuario(string codUsuario, DateTime fecha, [FromServices] IConfiguration config)
        {
            var list = new List<Models.DetallePedido>();

            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "SELECT DP.*, OM.nombre as nombreOpcion, DM.precioReal FROM DetallePedido DP inner join DetalleMenu DM on DM.idMenu = DP.idMenu and DM.idOpcionesMenu = DP.idOpcionesMenu inner join OpcionesMenu OM on OM.idOpcionesMenu = DM.idOpcionesMenu WHERE DP.fechaPedido = @FECHA AND DP.idUsuario = @USR";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new MySqlParameter("FECHA", fecha));
                    cmd.Parameters.Add(new MySqlParameter("USR", codUsuario));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obj = new Models.DetallePedido()
                            {
                                pedido = new Models.Pedido() { fecha = fecha, usuario = new Models.Usuario() { id = codUsuario, } },
                                detallePedido = new Models.DetalleMenu()
                                {
                                    menu = new Models.Menu() { id = Rutinas.intNul(reader, "idMenu") },
                                    opcion = new Models.OpcionesMenu() { id = Rutinas.intNul(reader, "idOpcionesMenu"), nombre = Rutinas.stringNul(reader, "nombreOpcion") },
                                    precioReal = Rutinas.intNul(reader, "precioReal")
                                },
                                cantidad = Rutinas.intNul(reader, "cantidad"),
                                total = Rutinas.intNul(reader, "cantidad") * Rutinas.intNul(reader, "precioReal")
                            };

                            list.Add(obj);
                        }
                    }
                }
            }

            return Ok(list);
        }

        [HttpPost]
        public ActionResult Post([FromServices] IConfiguration config, [FromBody] Models.DetallePedido value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();

                    string sql = "";

                    var cant = 0;
                    sql = "SELECT count(1) FROM Pedido WHERE idUsuario = @USR and fechaPedido = @FECHA";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("USR", value.pedido.usuario.id));
                        cmd.Parameters.Add(new MySqlParameter("FECHA", value.pedido.fecha));

                        cant = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    if (cant == 0)
                    {
                        sql = "INSERT INTO Pedido (idUsuario, fechaPedido, pagado) VALUES (@USR, @FECHA, 'N')";
                        using (var cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add(new MySqlParameter("USR", value.pedido.usuario.id));
                            cmd.Parameters.Add(new MySqlParameter("FECHA", value.pedido.fecha));

                            cmd.ExecuteNonQuery();
                        }
                    }

                    sql = "INSERT INTO DetallePedido (idMenu, idUsuario, fechaPedido, idOpcionesMenu, cantidad) VALUES (@IDMNU, @USR, @FECHA, @OPCION, @CANT)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("USR", value.pedido.usuario.id));
                        cmd.Parameters.Add(new MySqlParameter("FECHA", value.pedido.fecha));
                        cmd.Parameters.Add(new MySqlParameter("IDMNU", value.detallePedido.menu.id));
                        cmd.Parameters.Add(new MySqlParameter("OPCION", value.detallePedido.opcion.id));
                        cmd.Parameters.Add(new MySqlParameter("CANT", value.cantidad));

                        cant = cmd.ExecuteNonQuery();
                        if (cant == 1)
                        {
                            return Created("", value);
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

        [HttpPut]
        public ActionResult<IEnumerable<Models.DetallePedido>> Put([FromServices] IConfiguration config, [FromBody] Models.DetallePedido value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "UPDATE DetallePedido SET cantidad = @CANT WHERE idMenu = @IDMNU AND idUsuario = @USR AND fechaPedido = @FECHA AND idOpcionesMenu = @OPCION ";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("USR", value.pedido.usuario.id));
                        cmd.Parameters.Add(new MySqlParameter("FECHA", value.pedido.fecha));
                        cmd.Parameters.Add(new MySqlParameter("IDMNU", value.detallePedido.menu.id));
                        cmd.Parameters.Add(new MySqlParameter("OPCION", value.detallePedido.opcion.id));
                        cmd.Parameters.Add(new MySqlParameter("CANT", value.cantidad));

                        var cant = cmd.ExecuteNonQuery();
                        if (cant == 1)
                        {
                            return Accepted();
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

        [HttpDelete]
        public ActionResult Delete([FromServices] IConfiguration config, string codUsuario, DateTime fecha, int codMenu, int codOpcionMenu)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "DELETE FROM DetallePedido WHERE idMenu = @IDMNU AND idUsuario = @USR AND fechaPedido = @FECHA AND idOpcionesMenu = @OPCION";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("USR", codUsuario));
                        cmd.Parameters.Add(new MySqlParameter("FECHA", fecha));
                        cmd.Parameters.Add(new MySqlParameter("IDMNU", codMenu));
                        cmd.Parameters.Add(new MySqlParameter("OPCION", codOpcionMenu));

                        var cant = cmd.ExecuteNonQuery();
                        if (cant == 1)
                        {
                            return Accepted();
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