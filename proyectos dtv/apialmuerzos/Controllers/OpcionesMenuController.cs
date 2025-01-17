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
    public class OpcionesMenuController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Models.OpcionesMenu> Get(string codOpcionMenu, [FromServices] IConfiguration config)
        {
            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "SELECT * FROM OpcionesMenu WHERE idOpcionesMenu = @ID";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new MySqlParameter("ID", codOpcionMenu));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new Models.OpcionesMenu() { id = Rutinas.intNul(reader, "idOpcionesMenu"), nombre = Rutinas.stringNul(reader, "nombre"), precio = Rutinas.intNul(reader, "precioDefault") };

                            return Ok(user);
                        }
                    }
                }
            }

            return NotFound();
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<Models.OpcionesMenu>> Get([FromServices] IConfiguration config, string activo = "S")
        {
            var list = new List<Models.OpcionesMenu>();

            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "";
                if (activo == "ALL")
                {
                    sql = "SELECT * FROM OpcionesMenu";
                }
                else
                {
                    sql = "SELECT * FROM OpcionesMenu WHERE activo = '" + activo + "'";
                }
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new Models.OpcionesMenu() { id = Rutinas.intNul(reader, "idOpcionesMenu"), nombre = Rutinas.stringNul(reader, "nombre"), precio = Rutinas.intNul(reader, "precioDefault"), activo = Rutinas.stringNul(reader, "activo") };

                            list.Add(user);
                        }
                    }
                }
            }

            return Ok(list);
        }

        [HttpPost]
        public ActionResult<IEnumerable<Models.OpcionesMenu>> Post([FromServices] IConfiguration config, [FromBody] Models.OpcionesMenu value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();

                    int ultimo = 1;
                    string sql = "";
                    if (value.id == 0)
                    {
                        sql = "SELECT max(idOpcionesMenu) FROM OpcionesMenu";
                        using (var cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.CommandType = CommandType.Text;

                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ultimo = reader.GetInt32(0) + 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        ultimo = value.id;
                    }

                    sql = "INSERT INTO OpcionesMenu (idOpcionesMenu, nombre, precioDefault, activo) VALUES (@ID, @NOM, @PRECIO, @ACT)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        value.id = ultimo;
                        cmd.Parameters.Add(new MySqlParameter("ID", ultimo));
                        cmd.Parameters.Add(new MySqlParameter("NOM", value.nombre));
                        cmd.Parameters.Add(new MySqlParameter("PRECIO", value.precio));
                        cmd.Parameters.Add(new MySqlParameter("ACT", value.activo));

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
        public ActionResult<IEnumerable<Models.OpcionesMenu>> Put([FromServices] IConfiguration config, [FromBody] Models.OpcionesMenu value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "UPDATE OpcionesMenu SET nombre = @NOM, precioDefault = @PRECIO, activo = @ACT WHERE idOpcionesMenu = @ID";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", value.id));

                        cmd.Parameters.Add(new MySqlParameter("NOM", value.nombre));
                        cmd.Parameters.Add(new MySqlParameter("PRECIO", value.precio));
                        cmd.Parameters.Add(new MySqlParameter("ACT", value.activo));

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
        public ActionResult Delete([FromServices] IConfiguration config, string codOpcionMenu)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "DELETE FROM OpcionesMenu WHERE idOpcionesMenu = @ID";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", codOpcionMenu));

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