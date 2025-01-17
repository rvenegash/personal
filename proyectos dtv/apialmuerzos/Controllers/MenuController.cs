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
    public class MenuController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Models.Usuario> Get(int codMenu, [FromServices] IConfiguration config)
        {
            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "SELECT * FROM Menu WHERE idMenu = @ID";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new MySqlParameter("ID", codMenu));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obj = new Models.Menu() { id = Rutinas.intNul(reader, "idMenu"), fecha = Rutinas.datetimeNul(reader, "fecha"), activo = Rutinas.stringNul(reader, "activo"), cerrado = Rutinas.stringNul(reader, "cerrado") };

                            return Ok(obj);
                        }
                    }
                }
            }

            return NotFound();
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<Models.Menu>> Get([FromServices] IConfiguration config)
        {
            var list = new List<Models.Menu>();

            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "SELECT * FROM Menu WHERE activo = 'S' ";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obj = new Models.Menu() { id = Rutinas.intNul(reader, "idMenu"), fecha = Rutinas.datetimeNul(reader, "fecha"), activo = Rutinas.stringNul(reader, "activo"), cerrado = Rutinas.stringNul(reader, "cerrado") };

                            list.Add(obj);
                        }
                    }
                }
            }

            return Ok(list);
        }

        [HttpPost]
        public ActionResult<IEnumerable<Models.Menu>> Post([FromServices] IConfiguration config, [FromBody] Models.Menu value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "";
                    if (value.activo == "S")
                    {
                        sql = "UPDATE Menu SET activo = 'N' ";
                        using (var cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            var cant = cmd.ExecuteNonQuery();
                        }
                    }

                    int ultimo = 1;
                    if (value.id == 0)
                    {
                        sql = "select max(idMenu) from Menu";
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

                    sql = "INSERT INTO Menu (idMenu, fecha, activo, cerrado) VALUES (@ID, @FECHA, @ACTIVO, @CERRADO)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        value.id = ultimo;
                        cmd.Parameters.Add(new MySqlParameter("ID", ultimo));
                        cmd.Parameters.Add(new MySqlParameter("FECHA", value.fecha));
                        cmd.Parameters.Add(new MySqlParameter("ACTIVO", value.activo));
                        cmd.Parameters.Add(new MySqlParameter("CERRADO", value.cerrado));

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
        public ActionResult<IEnumerable<Models.Menu>> Put([FromServices] IConfiguration config, [FromBody] Models.Menu value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "UPDATE Menu SET fecha = @FECHA, activo = @ACTIVO, cerrado = @CERRADO WHERE idMenu = @ID";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", value.id));

                        cmd.Parameters.Add(new MySqlParameter("FECHA", value.fecha));
                        cmd.Parameters.Add(new MySqlParameter("ACTIV", value.activo));
                        cmd.Parameters.Add(new MySqlParameter("CERRADO", value.cerrado));

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
        public ActionResult Delete([FromServices] IConfiguration config, int codMenu)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "DELETE FROM Menu WHERE idMenu = @ID";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", codMenu));

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