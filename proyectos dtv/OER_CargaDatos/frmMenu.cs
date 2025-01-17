using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OER_CargaDatos
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        private void bActuDatosOER_Click(object sender, EventArgs e)
        {
            //OER_CargaDatos.actualizaAssetsOER.MainActualizaOer();
            //OER_CargaDatos.exportaAssets.MainExportaAssets();
            OER_CargaDatos.actualizaAssetsOER.MainActualizaOer(false);

        }

        private void frmMenu_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var frm = new frmAprobaciones();
            frm.ShowDialog();
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bCreaMasivo_Click(object sender, EventArgs e)
        {
            var frm = new frmCreaMasivos();
            frm.ShowDialog();
        }

        private void frmRealName_Click(object sender, EventArgs e)
        {
            var frm = new frmActRealName();
            frm.ShowDialog();
        }

        private void bConciliador_Click(object sender, EventArgs e)
        {
            var frm = new frmConciliador();
            frm.ShowDialog();

        }

        private void bAsocia_Click(object sender, EventArgs e)
        {
            var frm = new frmAsocia();
            frm.ShowDialog();

        }

        private void bAssetXML_Click(object sender, EventArgs e)
        {
            var frm = new frmAssetXml();
            frm.ShowDialog();

        }

        private void tbAsociaAAsset_Click(object sender, EventArgs e)
        {
            var frm = new frmAsociaAAsset();
            frm.ShowDialog();
        }
    }
}
