using MySql.Data.MySqlClient;
using OER_CargaDatos.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OER_CargaDatos
{
    public partial class frmConciliador : Form
    {
        List<OperacionConc> lOperaciones;

        public frmConciliador()
        {
            InitializeComponent();
        }

        private void frmConciliador_Load(object sender, EventArgs e)
        {
            OERHelper.crearServicio();
        }

        private void bCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bActualizar_Click(object sender, EventArgs e)
        {
            actualizarGrilla();
        }

        private void actualizarGrilla()
        {
            this.Cursor = Cursors.WaitCursor;

            lOperaciones = new List<OperacionConc>();

            var sql = "select o.asset_id, o.real_name " +
                "from operations_rel opr " +
                "inner join operations o on opr.PORT_TYPE || '.' || opr.operation_name = o.real_name  " +
                "where o.ASSET_ID is not null and opr.INVOKED_ASSET_ID not in ( " +
                "select a2.id " +
                "from ASSET_RELATION ar, asset a , asset a2  " +
                "where  ar.RELATION_TYPE = 50000 and ar.RELATED_ASSET_ID = a.id and ar.ASSET_ID = a2.id and ar.RELATED_PS = 'S' and  " +
                "a2.type = 50004 and a.type = 50004 and " +
                "a.id = o.ASSET_ID " +
                ") " +
                "union " +
                "select  a2.id, a2.real_name " +
                "from ASSET_RELATION ar, asset a , asset a2  " +
                "where  ar.RELATION_TYPE = 50000 and ar.RELATED_ASSET_ID = a.id and ar.ASSET_ID = a2.id and ar.RELATED_PS = 'P' and  " +
                "a2.type = 50004 and a.type = 50004 and " +
                "a.id not in (  " +
                "select opr.INVOKED_ASSET_ID  " +
                "from operations_rel opr " +
                "inner join operations o on opr.PORT_TYPE || '.' || opr.operation_name = o.real_name " +
                "where o.ASSET_ID = ar.ASSET_id ) ORDER BY 2";

            if (rbEnOer.Checked)
            {
                sql = "select distinct a.id as asset_id, a.REAL_NAME  " +
                    "from ASSET_RELATION ar, asset a , asset a2, asset_type at, asset_type at2 " +
                    "where  ar.RELATION_TYPE = 50000 and ar.RELATED_ASSET_ID = a.id and ar.ASSET_ID = a2.id and ar.RELATED_PS = 'P' and " +
                    "a.id not in ( " +
                    "select opr.INVOKED_ASSET_ID from operations_rel opr inner join operations o on opr.real_name = o.real_name " +
                    "where o.ASSET_ID = ar.ASSET_id ) and a2.type = at.id and a.type = at2.id and a.type = 50004 and a2.type = 50004 and a2.REAL_NAME is not null " +
                    "order by 2";
            }
            else
            {
                sql = "select distinct o.ASSET_ID , o.REAL_NAME " +
                    "from operations_rel opr inner join operations o on concat(opr.PORT_TYPE, '.', opr.operation_name) = o.real_name " +
                    "where opr.INVOKED_ASSET_ID not in ( " +
                    "select ar.RELATED_ASSET_ID from ASSET_RELATION ar  " +
                    "where ar.ASSET_id =  o.ASSET_ID and ar.RELATION_TYPE = 50000)  and o.ASSET_ID  is not null " +
                    "order by 2";
            }

            var mysqldb = ConfigurationManager.AppSettings["MySQL160"];

            using (var conn = new MySqlConnection(mysqldb))
            {
                conn.Open();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        if (!lOperaciones.Exists(m => m.Nombre == reader["real_name"].ToString()))
                        {
                            var nombre = string.Format("{1} ({0})", reader["asset_id"], reader["real_name"].ToString());
                            lOperaciones.Add(new OperacionConc() { Id = Convert.ToInt32(reader["asset_id"].ToString()), Nombre = nombre });
                        }
                    }
                    reader.Close();
                    reader = null;
                }
            }

            cbOperacion.DataSource = lOperaciones;

            this.Cursor = Cursors.Default;
        }

        private void cbOperacion_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cbOperacion.SelectedItem != null)
            {
                this.Cursor = Cursors.WaitCursor;

                var oper = (OperacionConc)cbOperacion.SelectedItem;

                tbOperacion.Text = oper.Nombre;

                //buscar relaciones en OER no en GIT
                var sql = "select a.id, a.real_name, ar.RELATION_TYPE  " +
                    "from ASSET_RELATION ar, asset a , asset a2  " +
                    "where ar.RELATION_TYPE = 50000 and ar.RELATED_ASSET_ID = a.id and ar.ASSET_ID = a2.id and ar.RELATED_PS = 'P' and  " +
                    "a2.type = 50004 and a.type = 50004 and " +
                    "a.id not in (  " +
                    "select opr.INVOKED_ASSET_ID  " +
                    "from operations_rel opr " +
                    "inner join operations o on opr.real_name = o.real_name " +
                    "where o.ASSET_ID = ar.ASSET_id ) and a2.id = " + oper.Id +
                    " order by 2";

                lbOerNoGit.Items.Clear();
                var mysqldb = ConfigurationManager.AppSettings["MySQL160"];

                using (var conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            lbOerNoGit.Items.Add(new RelacionConc()
                            {
                                Id = Convert.ToInt32(reader["id"].ToString()),
                                Nombre = reader["real_name"].ToString(),
                                RelType = Convert.ToInt32(reader["RELATION_TYPE"].ToString())
                            });
                        }
                        reader.Close();
                        reader = null;
                    }
                }

                //buscar relaciones en GIT no en OER
                sql = "select opr.INVOKED_ASSET_ID , concat( opr.INVOKED_PORT_TYPE ,'.' , opr.OPERATION_INVOKED) REAL_NAME " +
                    "from operations_rel opr " +
                    "inner join operations o on opr.real_name = o.real_name  " +
                    "where o.ASSET_ID is not null and opr.INVOKED_ASSET_ID not in ( " +
                    "select a2.id " +
                    "from ASSET_RELATION ar, asset a , asset a2  " +
                    "where  ar.RELATION_TYPE = 50000 and ar.RELATED_ASSET_ID = a.id and ar.ASSET_ID = a2.id and ar.RELATED_PS = 'S' and  " +
                    "a2.type = 50004 and a.type = 50004 and " +
                    "a.id = o.ASSET_ID " +
                    ") and o.ASSET_ID = " + oper.Id +
                    " order by 2";

                lbGitNoOer.Items.Clear();

                using (var conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            lbGitNoOer.Items.Add(new RelacionConc()
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("INVOKED_ASSET_ID")) ? 0 : Convert.ToInt32(reader["INVOKED_ASSET_ID"].ToString()),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("INVOKED_ASSET_ID")) ? "(*) " + reader["REAL_NAME"].ToString() : reader["REAL_NAME"].ToString(),
                                RelType = 0
                            });
                        }
                        reader.Close();
                        reader = null;
                    }
                }

                //buscar todas las relaciones en GIT
                sql = "select opr.INVOKED_ASSET_ID , concat( opr.INVOKED_PORT_TYPE ,'.' , opr.OPERATION_INVOKED) REAL_NAME " +
                    "from operations_rel opr " +
                    "inner join operations o on opr.real_name = o.real_name " +
                    "where o.ASSET_ID = " + oper.Id +
                    " order by 2";

                lbEnGit.Items.Clear();

                using (var conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            lbEnGit.Items.Add(new RelacionConc()
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("INVOKED_ASSET_ID")) ? 0 : Convert.ToInt32(reader["INVOKED_ASSET_ID"].ToString()),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("INVOKED_ASSET_ID")) ? "(*) " + reader["REAL_NAME"].ToString() : reader["REAL_NAME"].ToString(),
                                RelType = 0
                            });
                        }
                        reader.Close();
                        reader = null;
                    }
                }

                //buscar todas las relaciones en OER
                sql = "select a.id, a.real_name, ar.RELATION_TYPE " +
                    "from ASSET_RELATION ar, asset a , asset a2 " +
                    "where ar.RELATION_TYPE = 50000 and ar.RELATED_ASSET_ID = a.id and ar.ASSET_ID = a2.id and ar.RELATED_PS = 'P' and " +
                    "a2.type = 50004 and a.type = 50004 and a2.id = " + oper.Id +
                    " order by 2";

                lbEnOer.Items.Clear();

                using (var conn = new MySqlConnection(mysqldb))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            lbEnOer.Items.Add(new RelacionConc()
                            {
                                Id = Convert.ToInt32(reader["id"].ToString()),
                                Nombre = reader["real_name"].ToString(),
                                RelType = Convert.ToInt32(reader["RELATION_TYPE"].ToString())
                            });
                        }
                        reader.Close();
                        reader = null;
                    }
                }

                this.Cursor = Cursors.Default;
            }

        }

        private void bEliminarRel_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var oper = (OperacionConc)cbOperacion.SelectedItem;

            List<long> idRel = new List<long>();
            foreach (var item in lbOerNoGit.SelectedItems)
            {
                var rel = (RelacionConc)item;
                idRel.Add(rel.Id);
            }
            if (idRel.Count() > 0)
            {
                if (!OERHelper.borraAssetsRelated(oper.Id, 50000, idRel.ToArray()))//50000 Invokes
                {
                    MessageBox.Show("Error al borrar relación");
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void bCrearRel_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var oper = (OperacionConc)cbOperacion.SelectedItem;

            List<long> idRel = new List<long>();
            foreach (var item in lbGitNoOer.SelectedItems)
            {
                var rel = (RelacionConc)item;
                idRel.Add(rel.Id);
            }
            if (idRel.Count() > 0)
            {
                if (!OERHelper.creaAssetsRelated(oper.Id, 50000, idRel.ToArray())) //50000 Invokes
                {
                    MessageBox.Show("Error al crear relación");
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void bCrearAssets_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            var oper = (OperacionConc)cbOperacion.SelectedItem;
            var oradb = ConfigurationManager.AppSettings["MySQL160"];

            //pedir version
            foreach (var item in lbGitNoOer.SelectedItems)
            {
                long servId = 0;
                var version = "";
                var nomOper = "";
                var rel = (RelacionConc)item;
                //separar nombre
                var nombreIntfA = rel.Nombre.Split(' ', '.');
                if (nombreIntfA.Length == 3)
                {
                    nomOper = nombreIntfA[2];

                    //buscar nombre de servicio por interfaz
                    var sql = "select * from asset where REAL_NAME = '" + nombreIntfA[1] + "'";

                    using (var conn = new MySqlConnection(oradb))
                    {
                        conn.Open();

                        using (var cmd = new MySqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = sql;
                            cmd.CommandType = CommandType.Text;

                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                servId = Convert.ToInt32(reader["id"].ToString());
                                version = reader["version"].ToString();
                                break;
                            }
                            reader.Close();
                            reader = null;
                        }
                    }

                    if (servId > 0)
                    {
                        OERHelper.creaAssetContenido(nomOper, 50004, servId, version, "real-name", nombreIntfA[1] + "." + nomOper);
                    }
                }
            }

            this.Cursor = Cursors.Default;
        }

    }

    class RelacionConc
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public long RelType { get; set; }
    }
    class OperacionConc
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
    }

}
