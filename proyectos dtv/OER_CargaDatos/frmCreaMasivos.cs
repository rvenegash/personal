using OER_CargaDatos.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OER_CargaDatos
{
    public partial class frmCreaMasivos : Form
    {
        public frmCreaMasivos()
        {
            InitializeComponent();
        }

        private void bSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bSubmit_Click(object sender, EventArgs e)
        {
            if (cbTipoAssets.SelectedItem != null)
            {
                if (lbAssets.Items.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    progressBar1.Value = 0;
                    progressBar1.Maximum = lbAssets.Items.Count;

                    var tipo = (AssetType)cbTipoAssets.SelectedItem;
                    var servicioId = (cbServicios.SelectedItem) == null ? 0 : ((Asset)cbServicios.SelectedItem).Id;
                    var version = (cbVersion.SelectedItem) == null ? "1.0" : ((string)cbVersion.SelectedItem);

                    foreach (var item in lbAssets.Items)
                    {
                        OERHelper.creaAssetContenido((string)item, tipo.Id, servicioId, version, "", "");

                        progressBar1.Value++;
                    }
                    bSubmit.Enabled = false;
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void frmCreaMasivos_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            OERHelper.crearServicio();

            var atL = OERHelper.getAssetsType();
            cbTipoAssets.DisplayMember = "Name";
            cbTipoAssets.ValueMember = "Id";
            cbTipoAssets.DataSource = atL;

            var atS = OERHelper.getServicios();
            cbServicios.DisplayMember = "Name";
            cbServicios.ValueMember = "Id";
            cbServicios.DataSource = atS;

            this.Cursor = Cursors.Default;
        }

        private void bBuscar_Click(object sender, EventArgs e)
        {
            var sr = tbNombre.Text.Split('\n', '\r');
            foreach (var item in sr)
            {
                if (item != "")
                {
                    lbAssets.Items.Add(item);
                }
            }
            tbNombre.Text = "";
        }

        private void bLimpiarAssets_Click(object sender, EventArgs e)
        {
            lbAssets.Items.Clear();

            bSubmit.Enabled = true;
        }
    }
}
