using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winMantCarpetas
{
    public partial class Form1 : Form
    {
        MySqlConnection conn;
        public Form1()
        {
            InitializeComponent();
        }

        private void bCerrar_Click(object sender, EventArgs e)
        {
            conn.Close();
            this.Close();
        }

        private void bActualizado_Click(object sender, EventArgs e)
        {
            //grabar en bd el cambio
            var carpeta = (CarpetaRevisar)lbCarpetas.SelectedItem;

            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                //cmd.CommandText = "update carpetas_osb set revisar = '" + (cbRevisar.Checked ? "S" : "N") + "', fecha_actualizacion = CURRENT_DATE where tipo = '" + carpeta.Tipo + "' and carpeta = '" + carpeta.Carpeta + "' ";
                cmd.CommandText = "update carpetas_osb set revisar = ?revisar, fecha_actualizacion = ?fecha where tipo = ?tipo and carpeta = ?carpeta";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("?revisar", MySqlDbType.String).Value = (cbRevisar.Checked ? "S" : "N");
                cmd.Parameters.Add("?fecha", MySqlDbType.DateTime).Value = dateTimePicker1.Value;
                cmd.Parameters.Add("?tipo", MySqlDbType.String).Value = carpeta.Tipo;
                cmd.Parameters.Add("?carpeta", MySqlDbType.String).Value = carpeta.Carpeta;

                var update = cmd.ExecuteNonQuery();

            }
        }

        private void rbFront_CheckedChanged(object sender, EventArgs e)
        {
            cargar_carpetas("F");
        }

        private void tbBack_CheckedChanged(object sender, EventArgs e)
        {
            cargar_carpetas("B");
        }

        private void cargar_carpetas(string tipo)
        {
            lbCarpetas.Items.Clear();
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "select * from carpetas_osb where tipo = '" + tipo + "' ";
                cmd.CommandType = CommandType.Text;

                var reader = cmd.ExecuteReader();
                //grabar resultado en archivo
                while (reader.Read())
                {
                    lbCarpetas.Items.Add(new CarpetaRevisar() { Tipo = reader["tipo"].ToString(), Carpeta = reader["carpeta"].ToString(), Fecha = reader["fecha_actualizacion"].ToString(), Revisar = reader["revisar"].ToString() });
                }
                reader.Close();
                reader = null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            var mysqldb = ConfigurationManager.AppSettings["MySQL160"];
            conn = new MySqlConnection(mysqldb);
            conn.Open();

        }

        private void lbCarpetas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbCarpetas.SelectedItem != null)
            {
                var carpeta = (CarpetaRevisar)lbCarpetas.SelectedItem;
                cbRevisar.Checked = carpeta.Revisar == "S";
                DateTime fecha;
                if (DateTime.TryParse(carpeta.Fecha, out fecha))
                {
                    dateTimePicker1.Value = fecha;
                }
            }
        }
    }


    class CarpetaRevisar
    {
        public string Tipo { get; set; }
        public string Carpeta { get; set; }
        public string Fecha { get; set; }
        public string Revisar { get; set; }

        public string Nombre
        {
            get
            { return string.Format("{0} / {1} / {2}", Carpeta, Revisar, Fecha); }
        }
    }
}
