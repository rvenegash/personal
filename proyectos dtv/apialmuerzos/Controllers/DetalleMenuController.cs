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
    public class DetalleMenuController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Models.DetalleMenu> GetDetalleMenu(int codMenu, int codDetalleMenu, [FromServices] IConfiguration config)
        {
            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "SELECT D.*, O.nombre as nombreOpcion FROM DetalleMenu D INNER JOIN OpcionesMenu O on O.idOpcionesMenu = D.idOpcionesMenu WHERE D.idMenu = @ID AND D.idOpcionesMenu = @IDDET";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new MySqlParameter("ID", codMenu));
                    cmd.Parameters.Add(new MySqlParameter("IDDET", codDetalleMenu));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obj = new Models.DetalleMenu()
                            {
                                menu = new Models.Menu() { id = Rutinas.intNul(reader, "idMenu") },
                                opcion = new Models.OpcionesMenu() { id = Rutinas.intNul(reader, "idOpcionesMenu"), nombre = Rutinas.stringNul(reader, "nombreOpcion") },
                                precioReal = Rutinas.intNul(reader, "precioReal")
                            };

                            return Ok(obj);
                        }
                    }
                }
            }

            return NotFound();
        }

        [HttpGet]
        [Route("menu")]
        public ActionResult<IEnumerable<Models.DetalleMenu>> GetDetalleMenuDeMenu(int codMenu, [FromServices] IConfiguration config)
        {
            var list = new List<Models.DetalleMenu>();

            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "SELECT D.*, O.nombre as nombreOpcion FROM DetalleMenu D INNER JOIN OpcionesMenu O on O.idOpcionesMenu = D.idOpcionesMenu WHERE D.idMenu = @ID ";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new MySqlParameter("ID", codMenu));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obj = new Models.DetalleMenu()
                            {
                                menu = new Models.Menu() { id = Rutinas.intNul(reader, "idMenu") },
                                opcion = new Models.OpcionesMenu() { id = Rutinas.intNul(reader, "idOpcionesMenu"), nombre = Rutinas.stringNul(reader, "nombreOpcion") },
                                precioReal = Rutinas.intNul(reader, "precioReal")
                            };

                            list.Add(obj);
                        }
                    }
                }
            }

            return Ok(list);
        }

        [HttpPost]
        public ActionResult<IEnumerable<Models.DetalleMenu>> Post([FromServices] IConfiguration config, [FromBody] Models.DetalleMenu value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "INSERT INTO DetalleMenu (idMenu, idOpcionesMenu, precioReal) VALUES (@ID, @IDDET, @PRECIO)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", value.menu.id));
                        cmd.Parameters.Add(new MySqlParameter("IDDET", value.opcion.id));
                        cmd.Parameters.Add(new MySqlParameter("PRECIO", value.precioReal));

                        var cant = cmd.ExecuteNonQuery();
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
        public ActionResult<IEnumerable<Models.DetalleMenu>> Put([FromServices] IConfiguration config, [FromBody] Models.DetalleMenu value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "UPDATE DetalleMenu SET precioReal = @PRECIO WHERE idMenu = @ID AND idOpcionesMenu = @IDDET";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", value.menu.id));
                        cmd.Parameters.Add(new MySqlParameter("IDDET", value.opcion.id));

                        cmd.Parameters.Add(new MySqlParameter("PRECIO", value.precioReal));

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
        public ActionResult Delete([FromServices] IConfiguration config, string codMenu, string codDetalleMenu)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "DELETE FROM DetalleMenu WHERE idMenu = @ID AND idOpcionesMenu = @IDDET";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", codMenu));
                        cmd.Parameters.Add(new MySqlParameter("IDDET", codDetalleMenu));

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