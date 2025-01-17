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
    public class UsuarioController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Models.Usuario> Get(string codUsuario, [FromServices] IConfiguration config)
        {
            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "SELECT * FROM Usuario WHERE idUsuario = @ID";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new MySqlParameter("ID", codUsuario));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new Models.Usuario() { id = Rutinas.stringNul(reader, "idUsuario"), nombre = Rutinas.stringNul(reader, "nombre"), telefono = Rutinas.stringNul(reader, "telefono"), email = Rutinas.stringNul(reader, "email"), password = Rutinas.stringNul(reader, "password"), admin = Rutinas.stringNul(reader, "admin") };

                            return Ok(user);
                        }
                    }
                }
            }

            return NotFound();
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<Models.Usuario>> Get([FromServices] IConfiguration config)
        {
            var list = new List<Models.Usuario>();

            var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
            using (MySqlConnection conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                string sql = "SELECT * FROM Usuario";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new Models.Usuario() { id = Rutinas.stringNul(reader, "idUsuario"), nombre = Rutinas.stringNul(reader, "nombre"), telefono = Rutinas.stringNul(reader, "telefono"), email = Rutinas.stringNul(reader, "email"), password = Rutinas.stringNul(reader, "password"), admin = Rutinas.stringNul(reader, "admin") };

                            list.Add(user);
                        }
                    }
                }
            }

            return Ok(list);
        }

        [HttpPost]
        public ActionResult<IEnumerable<Models.Usuario>> Post([FromServices] IConfiguration config, [FromBody] Models.Usuario value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "INSERT INTO Usuario (idUsuario, nombre, telefono, email, password, admin) VALUES (@ID, @NOM, @TEL, @EMAIL, @PASSW, @ADMIN)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", value.id));
                        cmd.Parameters.Add(new MySqlParameter("NOM", value.nombre));
                        cmd.Parameters.Add(new MySqlParameter("TEL", value.telefono));
                        cmd.Parameters.Add(new MySqlParameter("EMAIL", value.email));
                        cmd.Parameters.Add(new MySqlParameter("PASSW", value.password));
                        cmd.Parameters.Add(new MySqlParameter("ADMIN", value.admin));

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
        public ActionResult<IEnumerable<Models.Usuario>> Put([FromServices] IConfiguration config, [FromBody] Models.Usuario value)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "UPDATE Usuario SET nombre = @NOM, telefono= @TEL, email = @EMAIL, password = @PASSW, admin = @ADMIN WHERE idUsuario = @ID";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", value.id));

                        cmd.Parameters.Add(new MySqlParameter("NOM", value.nombre));
                        cmd.Parameters.Add(new MySqlParameter("TEL", value.telefono));
                        cmd.Parameters.Add(new MySqlParameter("EMAIL", value.email));
                        cmd.Parameters.Add(new MySqlParameter("PASSW", value.password));
                        cmd.Parameters.Add(new MySqlParameter("ADMIN", value.admin));

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
        public ActionResult Delete([FromServices] IConfiguration config, string codUsuario)
        {
            if (ModelState.IsValid)
            {
                var mysqldb = config.GetSection("AppSettings").GetSection("dbconn").Value;
                using (MySqlConnection conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();
                    string sql = "DELETE FROM Usuario WHERE idUsuario = @ID";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new MySqlParameter("ID", codUsuario));

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